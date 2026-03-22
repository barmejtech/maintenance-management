using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class GroupRepository : Repository<TechnicianGroup>, IGroupRepository
{
    public GroupRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TechnicianGroup?> GetWithMembersAsync(Guid groupId)
        => await _dbSet
            .Include(g => g.Members.Where(m => !m.IsDeleted))
                .ThenInclude(m => m.Technician)
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted);

    public async Task AddMemberAsync(Guid groupId, Guid technicianId)
    {
        var existing = await _context.TechnicianGroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.TechnicianId == technicianId);

        if (existing is not null)
        {
            existing.IsDeleted = false;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            await _context.TechnicianGroupMembers.AddAsync(new TechnicianGroupMember
            {
                GroupId = groupId,
                TechnicianId = technicianId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid technicianId)
    {
        var member = await _context.TechnicianGroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.TechnicianId == technicianId && !m.IsDeleted);

        if (member is not null)
        {
            member.IsDeleted = true;
            member.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
