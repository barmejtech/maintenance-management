using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.EquipmentDigitalTwin;

public class EquipmentDigitalTwinDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentLocation { get; set; } = string.Empty;
    public EquipmentStatus CurrentStatus { get; set; }
    public double WearPercentage { get; set; }
    public double PerformanceScore { get; set; }
    public double? TemperatureCelsius { get; set; }
    public double? UsageHours { get; set; }
    public string? LastKnownIssue { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SimulationNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpsertDigitalTwinDto
{
    public Guid EquipmentId { get; set; }
    public EquipmentStatus CurrentStatus { get; set; }
    public double WearPercentage { get; set; }
    public double PerformanceScore { get; set; }
    public double? TemperatureCelsius { get; set; }
    public double? UsageHours { get; set; }
    public string? LastKnownIssue { get; set; }
    public string? SimulationNotes { get; set; }
}
