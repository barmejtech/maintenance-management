namespace Maintenance_management.application.DTOs.HVAC;

public class HVACSystemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public string CapacityUnit { get; set; } = string.Empty;
    public string RefrigerantType { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastInspectionDate { get; set; }
    public DateTime? NextInspectionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? EquipmentId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHVACSystemDto
{
    public string Name { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public string CapacityUnit { get; set; } = string.Empty;
    public string RefrigerantType { get; set; } = string.Empty;
    public DateTime? InstallationDate { get; set; }
    public DateTime? NextInspectionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? EquipmentId { get; set; }
}

public class UpdateHVACSystemDto : CreateHVACSystemDto
{
    public DateTime? LastInspectionDate { get; set; }
}
