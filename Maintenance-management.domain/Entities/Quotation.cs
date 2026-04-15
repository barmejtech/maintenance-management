using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class Quotation : BaseEntity
{
    public string QuotationNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public string? ClientPhone { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ValidUntil { get; set; }
    public int EstimatedDurationDays { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    public Guid? MaintenanceRequestId { get; set; }
    public MaintenanceRequest? MaintenanceRequest { get; set; }

    public Guid? ClientId { get; set; }
    public Client? Client { get; set; }

    public ICollection<QuotationLineItem> LineItems { get; set; } = new List<QuotationLineItem>();
}
