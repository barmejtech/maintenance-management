using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class Technician : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public TechnicianStatus Status { get; set; } = TechnicianStatus.Available;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? LastLocationUpdate { get; set; }

    public ICollection<TechnicianGroupMember> GroupMembers { get; set; } = new List<TechnicianGroupMember>();
    public ICollection<TaskOrder> Tasks { get; set; } = new List<TaskOrder>();
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
