using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IGroupRepository : IRepository<TechnicianGroup>
{
    Task<TechnicianGroup?> GetWithMembersAsync(Guid groupId);
    Task AddMemberAsync(Guid groupId, Guid technicianId);
    Task RemoveMemberAsync(Guid groupId, Guid technicianId);
}
