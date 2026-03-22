namespace Maintenance_management.domain.Entities;

public class TechnicianGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LeaderUserId { get; set; }

    public ICollection<TechnicianGroupMember> Members { get; set; } = new List<TechnicianGroupMember>();
    public ICollection<TaskOrder> Tasks { get; set; } = new List<TaskOrder>();
}
