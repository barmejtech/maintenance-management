using Maintenance_management.application.DTOs.EquipmentDigitalTwin;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class EquipmentDigitalTwinService : IEquipmentDigitalTwinService
{
    private readonly IEquipmentDigitalTwinRepository _twinRepo;
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly ITaskOrderRepository _taskOrderRepo;

    public EquipmentDigitalTwinService(
        IEquipmentDigitalTwinRepository twinRepo,
        IEquipmentRepository equipmentRepo,
        ITaskOrderRepository taskOrderRepo)
    {
        _twinRepo = twinRepo;
        _equipmentRepo = equipmentRepo;
        _taskOrderRepo = taskOrderRepo;
    }

    public async Task<IEnumerable<EquipmentDigitalTwinDto>> GetAllAsync()
    {
        var twins = await _twinRepo.GetAllWithEquipmentAsync();
        return twins.Select(MapToDto);
    }

    public async Task<EquipmentDigitalTwinDto?> GetByEquipmentIdAsync(Guid equipmentId)
    {
        var twin = await _twinRepo.GetByEquipmentIdAsync(equipmentId);
        return twin is null ? null : MapToDto(twin);
    }

    public async Task<EquipmentDigitalTwinDto> UpsertAsync(UpsertDigitalTwinDto dto)
    {
        var equipment = await _equipmentRepo.GetByIdAsync(dto.EquipmentId)
            ?? throw new KeyNotFoundException($"Equipment {dto.EquipmentId} not found.");

        var existing = await _twinRepo.GetByEquipmentIdAsync(dto.EquipmentId);
        EquipmentDigitalTwin twin;

        if (existing is not null)
        {
            existing.CurrentStatus = dto.CurrentStatus;
            existing.WearPercentage = dto.WearPercentage;
            existing.PerformanceScore = dto.PerformanceScore;
            existing.TemperatureCelsius = dto.TemperatureCelsius;
            existing.UsageHours = dto.UsageHours;
            existing.LastKnownIssue = dto.LastKnownIssue;
            existing.SimulationNotes = dto.SimulationNotes;
            existing.LastSyncedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            await _twinRepo.UpdateAsync(existing);
            twin = existing;
        }
        else
        {
            twin = new EquipmentDigitalTwin
            {
                EquipmentId = dto.EquipmentId,
                CurrentStatus = dto.CurrentStatus,
                WearPercentage = dto.WearPercentage,
                PerformanceScore = dto.PerformanceScore,
                TemperatureCelsius = dto.TemperatureCelsius,
                UsageHours = dto.UsageHours,
                LastKnownIssue = dto.LastKnownIssue,
                SimulationNotes = dto.SimulationNotes,
                LastSyncedAt = DateTime.UtcNow
            };
            await _twinRepo.AddAsync(twin);
        }

        twin.Equipment = equipment;
        return MapToDto(twin);
    }

    public async Task<EquipmentDigitalTwinDto?> SyncFromEquipmentAsync(Guid equipmentId)
    {
        var equipment = await _equipmentRepo.GetByIdAsync(equipmentId);
        if (equipment is null || equipment.IsDeleted) return null;

        var tasks = (await _taskOrderRepo.GetByEquipmentIdAsync(equipmentId))
            .Where(t => !t.IsDeleted)
            .ToList();

        double wearPercentage = ComputeWearPercentage(equipment, tasks.Count);
        double performanceScore = ComputePerformanceScore(equipment, tasks.Count);

        var dto = new UpsertDigitalTwinDto
        {
            EquipmentId = equipmentId,
            CurrentStatus = equipment.Status,
            WearPercentage = wearPercentage,
            PerformanceScore = performanceScore,
            LastKnownIssue = tasks
                .Where(t => t.Status == domain.Enums.TaskStatus.Completed)
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Select(t => t.Description)
                .FirstOrDefault()
        };

        return await UpsertAsync(dto);
    }

    // ── private helpers ──────────────────────────────────────────────

    private static double ComputeWearPercentage(Equipment equipment, int taskCount)
    {
        double wear = 0;

        if (equipment.InstallationDate.HasValue)
        {
            var ageYears = (DateTime.UtcNow - equipment.InstallationDate.Value).TotalDays / 365;
            wear += Math.Min(ageYears * 5, 40); // Up to 40% from age
        }

        wear += Math.Min(taskCount * 3, 40); // Up to 40% from task count

        if (equipment.Status == domain.Enums.EquipmentStatus.OutOfService) wear += 20;
        else if (equipment.Status == domain.Enums.EquipmentStatus.UnderMaintenance) wear += 10;

        return Math.Min(wear, 100);
    }

    private static double ComputePerformanceScore(Equipment equipment, int taskCount)
    {
        double score = 100;

        if (equipment.InstallationDate.HasValue)
        {
            var ageYears = (DateTime.UtcNow - equipment.InstallationDate.Value).TotalDays / 365;
            score -= Math.Min(ageYears * 3, 25);
        }

        score -= Math.Min(taskCount * 2, 30);

        if (equipment.Status == domain.Enums.EquipmentStatus.OutOfService) score -= 40;
        else if (equipment.Status == domain.Enums.EquipmentStatus.UnderMaintenance) score -= 15;

        return Math.Max(score, 0);
    }

    private static EquipmentDigitalTwinDto MapToDto(EquipmentDigitalTwin t) => new()
    {
        Id = t.Id,
        EquipmentId = t.EquipmentId,
        EquipmentName = t.Equipment?.Name ?? string.Empty,
        EquipmentLocation = t.Equipment?.Location ?? string.Empty,
        CurrentStatus = t.CurrentStatus,
        WearPercentage = t.WearPercentage,
        PerformanceScore = t.PerformanceScore,
        TemperatureCelsius = t.TemperatureCelsius,
        UsageHours = t.UsageHours,
        LastKnownIssue = t.LastKnownIssue,
        LastSyncedAt = t.LastSyncedAt,
        SimulationNotes = t.SimulationNotes,
        CreatedAt = t.CreatedAt
    };
}
