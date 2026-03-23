namespace Maintenance_management.domain.Entities;

public class EquipmentHealthPrediction : BaseEntity
{
    public Guid EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public DateTime PredictedFailureDate { get; set; }
    public double FailureProbability { get; set; }
    public string Recommendation { get; set; } = string.Empty;

    public int TotalInterventions { get; set; }
    public double AverageDaysBetweenFailures { get; set; }
    public double AverageDaysBetweenMaintenance { get; set; }
    public DateTime? LastAnalyzedAt { get; set; }
}
