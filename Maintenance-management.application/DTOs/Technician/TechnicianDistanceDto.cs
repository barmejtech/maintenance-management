namespace Maintenance_management.application.DTOs.Technician;

public class TechnicianDistanceDto
{
    public Guid TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public double TechnicianLatitude { get; set; }
    public double TechnicianLongitude { get; set; }
    public double ServiceLatitude { get; set; }
    public double ServiceLongitude { get; set; }
    public double DistanceKm { get; set; }
}
