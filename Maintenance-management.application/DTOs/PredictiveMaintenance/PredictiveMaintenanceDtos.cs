namespace Maintenance_management.application.DTOs.PredictiveMaintenance;

public class EquipmentHealthPredictionDto
{
    public Guid Id { get; set; }
    public Guid EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public DateTime PredictedFailureDate { get; set; }
    public double FailureProbability { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public int TotalInterventions { get; set; }
    public double AverageDaysBetweenFailures { get; set; }
    public double AverageDaysBetweenMaintenance { get; set; }
    public DateTime? LastAnalyzedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RunPredictionDto
{
    public Guid EquipmentId { get; set; }
}
