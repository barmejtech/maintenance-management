namespace Maintenance_management.domain.Entities;

public class QuotationLineItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

    public Guid QuotationId { get; set; }
    public Quotation Quotation { get; set; } = null!;
}
