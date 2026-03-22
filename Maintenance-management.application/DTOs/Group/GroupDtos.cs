namespace Maintenance_management.application.DTOs.Group;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LeaderUserId { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GroupDetailDto : GroupDto
{
    public IList<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();
}

public class GroupMemberDto
{
    public Guid TechnicianId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
}

public class CreateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LeaderUserId { get; set; }
}

public class UpdateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LeaderUserId { get; set; }
}

public class AddMemberDto
{
    public Guid TechnicianId { get; set; }
}
