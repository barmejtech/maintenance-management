namespace Maintenance_management.domain.Entities;

public enum UnitStatus
{
    Vacant = 0,
    Occupied = 1,
    UnderMaintenance = 2,
    Reserved = 3,
    UnderRenovation = 4,
    OwnerOccupied = 5,
    Rented = 6
}

public class Unit : BaseEntity
{
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Vacant;
    public decimal? ShareValue { get; set; }

    public Guid UnitTypeId { get; set; }
    public UnitType? UnitType { get; set; }

    public ICollection<UnitOwnership> OwnershipHistory { get; set; } = new List<UnitOwnership>();
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
