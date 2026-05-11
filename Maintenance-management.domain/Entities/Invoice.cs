using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    // EXISTING
    public Guid? TaskOrderId { get; set; }
    public TaskOrder? TaskOrder { get; set; }

    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }
    public decimal? PaidAmount { get; set; }

    public Guid? MaintenanceReportId { get; set; }
    public MaintenanceReport? MaintenanceReport { get; set; }


    // Bill to specific unit (for strata fees)
    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    // Bill to specific owner
    public Guid? UnitOwnershipId { get; set; }
    public UnitOwnership? UnitOwnership { get; set; }

    // Bill to tenant
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    // EXISTING
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}