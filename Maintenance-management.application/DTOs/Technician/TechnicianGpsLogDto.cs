namespace Maintenance_management.application.DTOs.Technician;

public class TechnicianGpsLogDto
{
    public Guid Id { get; set; }
    public Guid TechnicianId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime RecordedAt { get; set; }
}
