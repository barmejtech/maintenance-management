using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.Invoice;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid? TaskOrderId { get; set; }
    public string? TaskTitle { get; set; }
    public Guid? ClientId { get; set; }
    public string? ClientCompany { get; set; }
    public Guid? MaintenanceReportId { get; set; }
    public string? MaintenanceReportTitle { get; set; }
    public IList<InvoiceLineItemDto> LineItems { get; set; } = new List<InvoiceLineItemDto>();
    public DateTime CreatedAt { get; set; }
}

public class InvoiceLineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class CreateInvoiceDto
{
    public string ClientName { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
    public string? ClientAddress { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public Guid? TaskOrderId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? MaintenanceReportId { get; set; }
    public IList<CreateLineItemDto> LineItems { get; set; } = new List<CreateLineItemDto>();
}

public class CreateLineItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class UpdateInvoiceDto : CreateInvoiceDto
{
    public InvoiceStatus Status { get; set; }
}
