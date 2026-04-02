namespace Maintenance_management.application.DTOs.SparePart;

public class SparePartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public int QuantityInStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Supplier { get; set; }
    public string? StorageLocation { get; set; }
    public string? Notes { get; set; }
    public string? Photo1Url { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
    public bool IsLowStock => QuantityInStock <= MinimumStockLevel;
    public DateTime CreatedAt { get; set; }
}

public class CreateSparePartDto
{
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public int QuantityInStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Supplier { get; set; }
    public string? StorageLocation { get; set; }
    public string? Notes { get; set; }
    public string? Photo1Url { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
}

public class UpdateSparePartDto
{
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public int QuantityInStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Supplier { get; set; }
    public string? StorageLocation { get; set; }
    public string? Notes { get; set; }
    public string? Photo1Url { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
}

public class SparePartUsageDto
{
    public Guid Id { get; set; }
    public Guid SparePartId { get; set; }
    public string SparePartName { get; set; } = string.Empty;
    public Guid? TaskOrderId { get; set; }
    public string? TaskOrderTitle { get; set; }
    public int QuantityUsed { get; set; }
    public string? Notes { get; set; }
    public DateTime UsedAt { get; set; }
    public string UsedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateSparePartUsageDto
{
    public Guid SparePartId { get; set; }
    public Guid? TaskOrderId { get; set; }
    public int QuantityUsed { get; set; }
    public string? Notes { get; set; }
}
