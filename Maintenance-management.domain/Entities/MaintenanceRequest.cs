using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class MaintenanceRequest : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentDescription { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public MaintenanceRequestStatus Status { get; set; } = MaintenanceRequestStatus.Pending;
    public string? Notes { get; set; }

    // Review tracking
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    public Guid? TaskOrderId { get; set; }
    public TaskOrder? TaskOrder { get; set; }

    public Guid? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }


    // Link to specific property unit
    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    // Link to specific tenant (if rented)
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    // Link to specific owner (direct, if owner requested)
    public Guid? OwnerId { get; set; }
    public Owner? Owner { get; set; }

    // Link to ownership record (to know which owner at time of request)
    public Guid? UnitOwnershipId { get; set; }
    public UnitOwnership? UnitOwnership { get; set; }

    // EXISTING
    public ICollection<MaintenanceRequestAssignment> Assignments { get; set; } = new List<MaintenanceRequestAssignment>();
}