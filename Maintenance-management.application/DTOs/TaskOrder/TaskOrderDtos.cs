using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.TaskOrder;

public class TaskOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public domain.Enums.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public Guid? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public Guid? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskOrderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public domain.Enums.TaskStatus Status { get; set; } = domain.Enums.TaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Corrective;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? EquipmentId { get; set; }
}

public class UpdateTaskOrderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public domain.Enums.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public MaintenanceType MaintenanceType { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? EquipmentId { get; set; }
}
