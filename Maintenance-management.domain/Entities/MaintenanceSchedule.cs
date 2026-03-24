using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class MaintenanceSchedule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Preventive;
    public ScheduleFrequency Frequency { get; set; } = ScheduleFrequency.Monthly;
    public int FrequencyValue { get; set; } = 1;
    public DateTime? LastExecutedAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    public Guid? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public Guid? AssignedTechnicianId { get; set; }
    public Technician? AssignedTechnician { get; set; }

    public Guid? AssignedGroupId { get; set; }
    public TechnicianGroup? AssignedGroup { get; set; }
}
