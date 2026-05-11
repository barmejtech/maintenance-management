using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Unit>> GetAllAsync()
        => await _dbSet
            .Include(u => u.UnitType)
            .Where(u => !u.IsDeleted)
            .ToListAsync();

    public async Task<Unit?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(u => u.UnitType)
            .Include(u => u.OwnershipHistory.Where(oh => !oh.IsDeleted))
                .ThenInclude(oh => oh.Owner)
            .Include(u => u.Tenants.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

    public async Task<IEnumerable<Unit>> GetByIdsAsync(IEnumerable<Guid> ids)
        => await _dbSet
            .Where(u => !u.IsDeleted && ids.Contains(u.Id))
            .ToListAsync();

    public async Task<bool> UnitNumberExistsAsync(string unitNumber, Guid? excludeId = null)
        => await _dbSet.AnyAsync(u => !u.IsDeleted
            && u.UnitNumber == unitNumber
            && (!excludeId.HasValue || u.Id != excludeId.Value));
}
