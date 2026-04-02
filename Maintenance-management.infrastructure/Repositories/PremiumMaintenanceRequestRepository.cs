using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class PremiumMaintenanceRequestRepository : Repository<PremiumMaintenanceRequest>, IPremiumMaintenanceRequestRepository
{
    public PremiumMaintenanceRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PremiumMaintenanceRequest?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(r => r.Client)
            .Include(r => r.PremiumService)
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

    public async Task<IEnumerable<PremiumMaintenanceRequest>> GetByClientIdAsync(Guid clientId)
        => await _dbSet
            .Include(r => r.PremiumService)
            .Include(r => r.Payment)
            .Where(r => r.ClientId == clientId && !r.IsDeleted)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
}
