namespace Maintenance_management.domain.Entities;

public class Availability : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }
}
