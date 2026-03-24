using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.MaintenanceSchedule;

public class MaintenanceScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceType MaintenanceType { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; }
    public DateTime? LastExecutedAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public Guid? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public string? AssignedTechnicianName { get; set; }
    public Guid? AssignedGroupId { get; set; }
    public string? AssignedGroupName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMaintenanceScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceType MaintenanceType { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; } = 1;
    public DateTime? NextDueAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public Guid? EquipmentId { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? AssignedGroupId { get; set; }
}

public class UpdateMaintenanceScheduleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceType MaintenanceType { get; set; }
    public ScheduleFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; } = 1;
    public DateTime? LastExecutedAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public Guid? EquipmentId { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public Guid? AssignedGroupId { get; set; }
}
