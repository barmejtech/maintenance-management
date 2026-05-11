namespace Maintenance_management.application.DTOs.UnitType;

public class UnitTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DefaultSizeSqm { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUnitTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DefaultSizeSqm { get; set; }
}

public class UpdateUnitTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DefaultSizeSqm { get; set; }
}
