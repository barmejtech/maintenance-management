using Maintenance_management.application.DTOs.AdminDashboard;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IMaintenanceRequestRepository _requestRepo;

    public AdminDashboardService(IMaintenanceRequestRepository requestRepo)
    {
        _requestRepo = requestRepo;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var requests = await _requestRepo.GetAllAsync();
        var list = requests.Where(r => !r.IsDeleted).ToList();

        // ── rows ────────────────────────────────────────────────────────────────
        var rows = list.Select(r => new AdminDashboardRowDto
        {
            RequestId   = r.Id,
            ClientName  = r.Client?.Name ?? string.Empty,
            DateRequest = r.RequestDate,
            Technicians = string.Join(", ", r.Assignments
                .Where(a => !a.IsDeleted && a.Technician != null)
                .Select(a => $"{a.Technician!.FirstName} {a.Technician.LastName}")),
            DateOfTask  = r.TaskOrder?.ScheduledDate,
            Status      = r.Status,
            Price       = r.Invoice?.TotalAmount,
            IsPaid      = r.Invoice?.Status == InvoiceStatus.Paid,
            DatePay     = r.Invoice?.PaidDate
        }).ToList();

        // ── completed tasks per day (last 30 days) ───────────────────────────
        var cutoff = DateTime.UtcNow.Date.AddDays(-29);
        var completedRequests = list
            .Where(r => r.Status == MaintenanceRequestStatus.Completed);

        // Use TaskOrder.CompletedDate when available, otherwise RequestDate
        var dailyStats = completedRequests
            .Select(r => (r.TaskOrder?.CompletedDate ?? r.ReviewedAt ?? r.RequestDate).Date)
            .Where(d => d >= cutoff)
            .GroupBy(d => d)
            .Select(g => new DailyStatDto { Date = g.Key, Count = g.Count() })
            .OrderBy(d => d.Date)
            .ToList();

        // ── weekly average ────────────────────────────────────────────────────
        double weeklyAverage = 0;
        if (completedRequests.Any())
        {
            var completedDates = completedRequests
                .Select(r => (r.TaskOrder?.CompletedDate ?? r.ReviewedAt ?? r.RequestDate).Date)
                .ToList();

            var earliest = completedDates.Min();
            var latest   = completedDates.Max();
            var totalWeeks = Math.Max(1, ((latest - earliest).TotalDays + 7) / 7.0);
            weeklyAverage = completedRequests.Count() / totalWeeks;
        }

        return new AdminDashboardDto
        {
            Rows         = rows,
            DailyStats   = dailyStats,
            WeeklyAverage = Math.Round(weeklyAverage, 2)
        };
    }
}
