namespace Maintenance_management.domain.Entities;

public class SparePart : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public int QuantityInStock { get; set; } = 0;
    public int MinimumStockLevel { get; set; } = 0;
    public decimal UnitPrice { get; set; } = 0;
    public string? Supplier { get; set; }
    public string? StorageLocation { get; set; }
    public string? Notes { get; set; }
    public string? Photo1Url { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }

    public ICollection<SparePartUsage> Usages { get; set; } = new List<SparePartUsage>();
}
