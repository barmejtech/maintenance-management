using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class PremiumServiceRepository : Repository<PremiumService>, IPremiumServiceRepository
{
    public PremiumServiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<PremiumService>> GetActiveAsync()
        => await _dbSet
            .Where(s => !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
}
