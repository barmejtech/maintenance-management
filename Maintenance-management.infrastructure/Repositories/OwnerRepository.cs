using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class OwnerRepository : Repository<Owner>, IOwnerRepository
{
    public OwnerRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Owner>> GetAllAsync()
        => await _dbSet
            .Include(o => o.UnitOwnerships.Where(h => !h.IsDeleted))
                .ThenInclude(h => h.Unit)
            .Where(o => !o.IsDeleted)
            .ToListAsync();

    public async Task<Owner?> GetWithHistoryAsync(Guid id)
        => await _dbSet
            .Include(o => o.UnitOwnerships.Where(h => !h.IsDeleted))
                .ThenInclude(h => h.Unit)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
}
