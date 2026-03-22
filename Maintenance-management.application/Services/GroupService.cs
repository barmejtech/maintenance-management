using Maintenance_management.application.DTOs.Group;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repo;

    public GroupService(IGroupRepository repo) => _repo = repo;

    public async Task<IEnumerable<GroupDto>> GetAllAsync()
    {
        var groups = await _repo.GetAllAsync();
        return groups.Where(g => !g.IsDeleted).Select(g => new GroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            LeaderUserId = g.LeaderUserId,
            MemberCount = g.Members.Count(m => !m.IsDeleted),
            CreatedAt = g.CreatedAt
        });
    }

    public async Task<GroupDetailDto?> GetByIdAsync(Guid id)
    {
        var group = await _repo.GetWithMembersAsync(id);
        if (group is null || group.IsDeleted) return null;

        return new GroupDetailDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            LeaderUserId = group.LeaderUserId,
            MemberCount = group.Members.Count(m => !m.IsDeleted),
            CreatedAt = group.CreatedAt,
            Members = group.Members
                .Where(m => !m.IsDeleted && !m.Technician.IsDeleted)
                .Select(m => new GroupMemberDto
                {
                    TechnicianId = m.TechnicianId,
                    FullName = $"{m.Technician.FirstName} {m.Technician.LastName}",
                    Specialization = m.Technician.Specialization
                }).ToList()
        };
    }

    public async Task<GroupDto> CreateAsync(CreateGroupDto dto)
    {
        var entity = new TechnicianGroup
        {
            Name = dto.Name,
            Description = dto.Description,
            LeaderUserId = dto.LeaderUserId
        };

        var created = await _repo.AddAsync(entity);
        return new GroupDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            LeaderUserId = created.LeaderUserId,
            MemberCount = 0,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<GroupDto?> UpdateAsync(Guid id, UpdateGroupDto dto)
    {
        var group = await _repo.GetByIdAsync(id);
        if (group is null || group.IsDeleted) return null;

        group.Name = dto.Name;
        group.Description = dto.Description;
        group.LeaderUserId = dto.LeaderUserId;
        group.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(group);
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            LeaderUserId = group.LeaderUserId,
            MemberCount = group.Members.Count(m => !m.IsDeleted),
            CreatedAt = group.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var group = await _repo.GetByIdAsync(id);
        if (group is null || group.IsDeleted) return false;

        group.IsDeleted = true;
        group.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(group);
        return true;
    }

    public async Task<bool> AddMemberAsync(Guid groupId, Guid technicianId)
    {
        try
        {
            await _repo.AddMemberAsync(groupId, technicianId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveMemberAsync(Guid groupId, Guid technicianId)
    {
        try
        {
            await _repo.RemoveMemberAsync(groupId, technicianId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
