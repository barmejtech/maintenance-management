namespace Maintenance_management.domain.Entities;

public class TechnicianGpsLog : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician? Technician { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
