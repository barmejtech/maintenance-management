using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class UnitOwnershipRepository : Repository<UnitOwnership>, IUnitOwnershipRepository
{
    public UnitOwnershipRepository(ApplicationDbContext context) : base(context) { }

    public async Task<UnitOwnership?> GetActiveByUnitAsync(Guid unitId)
        => await _dbSet
            .FirstOrDefaultAsync(h => h.UnitId == unitId && h.IsActive && !h.IsDeleted);

    public async Task<IEnumerable<UnitOwnership>> GetByUnitAsync(Guid unitId)
        => await _dbSet
            .Include(h => h.Owner)
            .Where(h => h.UnitId == unitId && !h.IsDeleted)
            .OrderByDescending(h => h.PurchaseDate)
            .ToListAsync();
}
