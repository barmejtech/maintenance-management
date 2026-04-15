using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.Quotation;

public class QuotationDto
{
    public Guid Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public string? ClientPhone { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public int EstimatedDurationDays { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public QuotationStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public Guid? MaintenanceRequestId { get; set; }
    public string? MaintenanceRequestTitle { get; set; }
    public Guid? ClientId { get; set; }
    public string? ClientCompany { get; set; }
    public IList<QuotationLineItemDto> LineItems { get; set; } = new List<QuotationLineItemDto>();
    public DateTime CreatedAt { get; set; }
}

public class QuotationLineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class CreateQuotationDto
{
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public string? ClientPhone { get; set; }
    public DateTime? ValidUntil { get; set; }
    public int EstimatedDurationDays { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public Guid? MaintenanceRequestId { get; set; }
    public Guid? ClientId { get; set; }
    public IList<CreateQuotationLineItemDto> LineItems { get; set; } = new List<CreateQuotationLineItemDto>();
}

public class CreateQuotationLineItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateQuotationDto : CreateQuotationDto
{
    public QuotationStatus Status { get; set; }
}
