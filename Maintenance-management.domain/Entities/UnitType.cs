namespace Maintenance_management.domain.Entities;

public class UnitType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DefaultSizeSqm { get; set; }

    public ICollection<Unit> Units { get; set; } = new List<Unit>();
}
