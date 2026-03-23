namespace Maintenance_management.application.DTOs.TechnicianPerformance;

public class TechnicianPerformanceScoreDto
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public double AverageInterventionTimeMinutes { get; set; }
    public double SuccessRate { get; set; }
    public double CustomerSatisfactionScore { get; set; }
    public double OnTimeRate { get; set; }
    public int TotalTasksCompleted { get; set; }
    public int TotalTasksDelayed { get; set; }
    public DateTime LastCalculatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateCustomerSatisfactionDto
{
    public double Score { get; set; }
}

public class SmartDispatchRequestDto
{
    public Guid TaskOrderId { get; set; }
}

public class SmartDispatchResultDto
{
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double DistanceScore { get; set; }
    public double PerformanceScore { get; set; }
    public double OverallScore { get; set; }
    public string AssignmentReason { get; set; } = string.Empty;
}
