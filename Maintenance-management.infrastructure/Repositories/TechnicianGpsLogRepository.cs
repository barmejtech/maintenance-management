using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class TechnicianGpsLogRepository : Repository<TechnicianGpsLog>, ITechnicianGpsLogRepository
{
    public TechnicianGpsLogRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TechnicianGpsLog>> GetByTechnicianIdAsync(Guid technicianId)
        => await _dbSet
            .Where(l => l.TechnicianId == technicianId && !l.IsDeleted)
            .OrderByDescending(l => l.RecordedAt)
            .ToListAsync();

    public async Task<TechnicianGpsLog?> GetLatestByTechnicianIdAsync(Guid technicianId)
        => await _dbSet
            .Where(l => l.TechnicianId == technicianId && !l.IsDeleted)
            .OrderByDescending(l => l.RecordedAt)
            .FirstOrDefaultAsync();
}
