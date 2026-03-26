using Maintenance_management.application.Interfaces;

namespace Maintenance_management.api.Services;

/// <summary>
/// Background service that runs daily and sends notifications for maintenance schedules due today.
/// </summary>
public class ScheduleTodayNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduleTodayNotificationService> _logger;

    public ScheduleTodayNotificationService(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduleTodayNotificationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run immediately on startup, then every 24 hours
        while (!stoppingToken.IsCancellationRequested)
        {
            await NotifyTodaySchedulesAsync(stoppingToken);

            // Wait until the same time tomorrow
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1).AddHours(6); // 06:00 UTC next day
            var delay = nextRun - now;
            if (delay < TimeSpan.FromMinutes(1)) delay = TimeSpan.FromHours(24);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task NotifyTodaySchedulesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var scheduleService = scope.ServiceProvider.GetRequiredService<IMaintenanceScheduleService>();
            var technicianService = scope.ServiceProvider.GetRequiredService<ITechnicianService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var todayStart = DateTime.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1);

            var allSchedules = await scheduleService.GetActiveSchedulesAsync();
            var todaySchedules = allSchedules
                .Where(s => s.NextDueAt.HasValue &&
                            s.NextDueAt.Value >= todayStart &&
                            s.NextDueAt.Value < todayEnd)
                .ToList();

            foreach (var schedule in todaySchedules)
            {
                var entityId = schedule.Id.ToString();
                const string entityType = "MaintenanceSchedule";

                await notificationService.SendToRoleAsync("Admin",
                    "Maintenance Due Today",
                    $"Maintenance schedule \"{schedule.Name}\" is due today.",
                    "warning", entityId, entityType);

                await notificationService.SendToRoleAsync("Manager",
                    "Maintenance Due Today",
                    $"Maintenance schedule \"{schedule.Name}\" is due today.",
                    "warning", entityId, entityType);

                if (schedule.AssignedTechnicianId.HasValue)
                {
                    var technician = await technicianService.GetByIdAsync(schedule.AssignedTechnicianId.Value);
                    if (technician is not null)
                    {
                        await notificationService.SendToUserAsync(technician.UserId,
                            "Maintenance Due Today",
                            $"Your maintenance schedule \"{schedule.Name}\" is due today.",
                            "warning", entityId, entityType);
                    }
                }
            }

            if (todaySchedules.Count > 0)
                _logger.LogInformation("Sent today's schedule notifications for {Count} schedule(s).", todaySchedules.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending today's maintenance schedule notifications.");
        }
    }
}
