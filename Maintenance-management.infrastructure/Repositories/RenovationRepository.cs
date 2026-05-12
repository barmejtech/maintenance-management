using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class RenovationRepository : Repository<Renovation>, IRenovationRepository
{
    public RenovationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Renovation?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(r => r.Unit)
            .Include(r => r.Expenses)
                .ThenInclude(e => e.Vendor)
            .Include(r => r.Documents)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

    public async Task<IEnumerable<Renovation>> GetByUnitIdAsync(Guid unitId)
        => await _dbSet
            .Include(r => r.Unit)
            .Where(r => r.UnitId == unitId && !r.IsDeleted)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<Renovation>> GetByStatusAsync(RenovationStatus status)
        => await _dbSet
            .Include(r => r.Unit)
            .Where(r => r.Status == status && !r.IsDeleted)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<Renovation>> GetInProgressAsync()
        => await _dbSet
            .Include(r => r.Unit)
            .Where(r => r.Status == RenovationStatus.InProgress && !r.IsDeleted)
            .OrderBy(r => r.StartDate)
            .ToListAsync();
}
