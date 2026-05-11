namespace Maintenance_management.application.DTOs.Owner;

public class OwnerUnitHistoryDto
{
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public decimal OwnershipPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; }
}

public class OwnerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<OwnerUnitHistoryDto> OwnershipHistory { get; set; } = [];
}

public class CreateOwnerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class UpdateOwnerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class TransferOwnershipDto
{
    public Guid UnitId { get; set; }
    public decimal OwnershipPercentage { get; set; } = 100m;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
}
