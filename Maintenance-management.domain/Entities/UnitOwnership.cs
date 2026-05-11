namespace Maintenance_management.domain.Entities;

public class UnitOwnership : BaseEntity
{
    public Guid UnitId { get; set; }
    public Unit? Unit { get; set; }

    public Guid OwnerId { get; set; }
    public Owner? Owner { get; set; }

    public decimal OwnershipPercentage { get; set; } = 100m;
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; } = true;
}
