namespace Maintenance_management.domain.Entities;

public class InvoiceLineItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }

    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
