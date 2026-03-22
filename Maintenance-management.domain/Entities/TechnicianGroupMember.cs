namespace Maintenance_management.domain.Entities;

public class TechnicianGroupMember : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;
    public Guid GroupId { get; set; }
    public TechnicianGroup Group { get; set; } = null!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
