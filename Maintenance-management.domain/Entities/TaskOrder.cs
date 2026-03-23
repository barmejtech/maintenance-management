using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class TaskOrder : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public MaintenanceType MaintenanceType { get; set; } = MaintenanceType.Corrective;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    // Intervention Proof (Feature 2)
    public double? ArrivalLatitude { get; set; }
    public double? ArrivalLongitude { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public string? ProofPhotoUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
    public bool IsGpsValidated { get; set; } = false;

    public Guid? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public Guid? GroupId { get; set; }
    public TechnicianGroup? Group { get; set; }

    public Guid? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<MaintenanceReport> Reports { get; set; } = new List<MaintenanceReport>();
    public Invoice? Invoice { get; set; }
}
