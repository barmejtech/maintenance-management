namespace Maintenance_management.application.DTOs.TravelEstimation;

public class TravelEstimationResultDto
{
    public double DistanceKm { get; set; }
    public double DurationMinutes { get; set; }
    public string FormattedDistance { get; set; } = string.Empty;
    public string FormattedDuration { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public bool TechnicianHasLocation { get; set; }
}
