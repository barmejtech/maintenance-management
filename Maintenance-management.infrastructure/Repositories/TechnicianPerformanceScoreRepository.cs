using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class TechnicianPerformanceScoreRepository : Repository<TechnicianPerformanceScore>, ITechnicianPerformanceScoreRepository
{
    public TechnicianPerformanceScoreRepository(ApplicationDbContext context) : base(context) { }

    public async Task<TechnicianPerformanceScore?> GetByTechnicianIdAsync(Guid technicianId)
        => await _dbSet
            .Include(s => s.Technician)
            .FirstOrDefaultAsync(s => s.TechnicianId == technicianId && !s.IsDeleted);

    public override async Task<IEnumerable<TechnicianPerformanceScore>> GetAllAsync()
        => await _dbSet
            .Include(s => s.Technician)
            .Where(s => !s.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<TechnicianPerformanceScore>> GetTopPerformersAsync(int count)
        => await _dbSet
            .Include(s => s.Technician)
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.SuccessRate)
            .Take(count)
            .ToListAsync();
}
