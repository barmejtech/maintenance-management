using Maintenance_management.domain.Entities;

namespace Maintenance_management.application.DTOs.Unit;

public class UnitOwnershipHistoryDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal OwnershipPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; }
}

public class UnitDto
{
    public Guid Id { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; }
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
    public string UnitTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<UnitOwnershipHistoryDto> OwnershipHistory { get; set; } = [];
}

public class CreateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Vacant;
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
}

public class UpdateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Vacant;
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
}

public class UnitMassUpdateDto
{
    public IEnumerable<Guid> UnitIds { get; set; } = [];
    public UnitStatus? Status { get; set; }
    public int? Floor { get; set; }
}
