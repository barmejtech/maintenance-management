using Maintenance_management.application.DTOs.MaintenanceSchedule;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class MaintenanceScheduleService : IMaintenanceScheduleService
{
    private readonly IMaintenanceScheduleRepository _repo;

    public MaintenanceScheduleService(IMaintenanceScheduleRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<MaintenanceScheduleDto>> GetAllAsync()
    {
        var schedules = await _repo.GetAllAsync();
        return schedules.Select(MapToDto);
    }

    public async Task<MaintenanceScheduleDto?> GetByIdAsync(Guid id)
    {
        var schedule = await _repo.GetByIdAsync(id);
        return schedule is null ? null : MapToDto(schedule);
    }

    public async Task<IEnumerable<MaintenanceScheduleDto>> GetByEquipmentIdAsync(Guid equipmentId)
    {
        var schedules = await _repo.GetByEquipmentIdAsync(equipmentId);
        return schedules.Select(MapToDto);
    }

    public async Task<IEnumerable<MaintenanceScheduleDto>> GetActiveSchedulesAsync()
    {
        var schedules = await _repo.GetActiveSchedulesAsync();
        return schedules.Select(MapToDto);
    }

    public async Task<IEnumerable<MaintenanceScheduleDto>> GetOverdueSchedulesAsync()
    {
        var schedules = await _repo.GetOverdueSchedulesAsync();
        return schedules.Select(MapToDto);
    }

    public async Task<IEnumerable<MaintenanceScheduleDto>> GetDueSoonAsync(int withinDays = 7)
    {
        var schedules = await _repo.GetDueSoonAsync(withinDays);
        return schedules.Select(MapToDto);
    }

    public async Task<MaintenanceScheduleDto> CreateAsync(CreateMaintenanceScheduleDto dto, string createdByUserId)
    {
        var schedule = new MaintenanceSchedule
        {
            Name = dto.Name,
            Description = dto.Description,
            MaintenanceType = dto.MaintenanceType,
            Frequency = dto.Frequency,
            FrequencyValue = dto.FrequencyValue,
            NextDueAt = dto.NextDueAt,
            IsActive = dto.IsActive,
            Notes = dto.Notes,
            EquipmentId = dto.EquipmentId,
            AssignedTechnicianId = dto.AssignedTechnicianId,
            AssignedGroupId = dto.AssignedGroupId,
            CreatedByUserId = createdByUserId
        };

        var created = await _repo.AddAsync(schedule);
        return MapToDto(created);
    }

    public async Task<MaintenanceScheduleDto?> UpdateAsync(Guid id, UpdateMaintenanceScheduleDto dto)
    {
        var schedule = await _repo.GetByIdAsync(id);
        if (schedule is null) return null;

        schedule.Name = dto.Name;
        schedule.Description = dto.Description;
        schedule.MaintenanceType = dto.MaintenanceType;
        schedule.Frequency = dto.Frequency;
        schedule.FrequencyValue = dto.FrequencyValue;
        schedule.LastExecutedAt = dto.LastExecutedAt;
        schedule.NextDueAt = dto.NextDueAt;
        schedule.IsActive = dto.IsActive;
        schedule.Notes = dto.Notes;
        schedule.EquipmentId = dto.EquipmentId;
        schedule.AssignedTechnicianId = dto.AssignedTechnicianId;
        schedule.AssignedGroupId = dto.AssignedGroupId;
        schedule.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(schedule);
        return MapToDto(schedule);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _repo.ExistsAsync(id)) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    private static MaintenanceScheduleDto MapToDto(MaintenanceSchedule s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
        MaintenanceType = s.MaintenanceType,
        Frequency = s.Frequency,
        FrequencyValue = s.FrequencyValue,
        LastExecutedAt = s.LastExecutedAt,
        NextDueAt = s.NextDueAt,
        IsActive = s.IsActive,
        Notes = s.Notes,
        CreatedByUserId = s.CreatedByUserId,
        EquipmentId = s.EquipmentId,
        EquipmentName = s.Equipment?.Name,
        AssignedTechnicianId = s.AssignedTechnicianId,
        AssignedTechnicianName = s.AssignedTechnician is not null
            ? $"{s.AssignedTechnician.FirstName} {s.AssignedTechnician.LastName}"
            : null,
        AssignedGroupId = s.AssignedGroupId,
        AssignedGroupName = s.AssignedGroup?.Name,
        CreatedAt = s.CreatedAt
    };
}
