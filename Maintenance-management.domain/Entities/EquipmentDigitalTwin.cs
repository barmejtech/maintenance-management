using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class EquipmentDigitalTwin : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public EquipmentStatus CurrentStatus { get; set; } = EquipmentStatus.Operational;
    public double WearPercentage { get; set; }
    public double PerformanceScore { get; set; } = 100;
    public double? TemperatureCelsius { get; set; }
    public double? UsageHours { get; set; }
    public string? LastKnownIssue { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SimulationNotes { get; set; }
}
