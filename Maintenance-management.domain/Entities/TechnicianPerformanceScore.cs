namespace Maintenance_management.domain.Entities;

public class TechnicianPerformanceScore : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public double AverageInterventionTimeMinutes { get; set; }
    public double SuccessRate { get; set; }
    public double CustomerSatisfactionScore { get; set; }
    public double OnTimeRate { get; set; }
    public int TotalTasksCompleted { get; set; }
    public int TotalTasksDelayed { get; set; }
    public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;
}
