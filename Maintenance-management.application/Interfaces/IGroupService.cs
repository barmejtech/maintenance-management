using Maintenance_management.application.DTOs.Group;

namespace Maintenance_management.application.Interfaces;

public interface IGroupService
{
    Task<IEnumerable<GroupDto>> GetAllAsync();
    Task<GroupDetailDto?> GetByIdAsync(Guid id);
    Task<GroupDto> CreateAsync(CreateGroupDto dto);
    Task<GroupDto?> UpdateAsync(Guid id, UpdateGroupDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AddMemberAsync(Guid groupId, Guid technicianId);
    Task<bool> RemoveMemberAsync(Guid groupId, Guid technicianId);
}
