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

    // EXISTING
    public ICollection<UnitOwnership> OwnershipHistory { get; set; } = new List<UnitOwnership>();
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    public ICollection<Renovation> Renovations { get; set; } = new List<Renovation>();
}