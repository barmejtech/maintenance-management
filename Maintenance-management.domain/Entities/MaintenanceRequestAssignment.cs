namespace Maintenance_management.domain.Entities;

public class MaintenanceRequestAssignment : BaseEntity
{
    public Guid MaintenanceRequestId { get; set; }
    public MaintenanceRequest? MaintenanceRequest { get; set; }

    public Guid TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public string AssignedByUserId { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
