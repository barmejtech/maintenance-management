using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Tenant>> GetAllAsync()
        => await _dbSet
            .Include(t => t.Unit)
            .Where(t => !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Tenant>> GetByUnitIdAsync(Guid unitId)
        => await _dbSet
            .Where(t => !t.IsDeleted && t.UnitId == unitId)
            .OrderByDescending(t => t.LeaseStartDate)
            .ToListAsync();
}
