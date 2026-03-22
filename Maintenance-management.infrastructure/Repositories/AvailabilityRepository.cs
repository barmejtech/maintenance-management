using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class AvailabilityRepository : Repository<Availability>, IAvailabilityRepository
{
    public AvailabilityRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Availability>> GetByTechnicianIdAsync(Guid technicianId)
        => await _dbSet
            .Include(a => a.Technician)
            .Where(a => a.TechnicianId == technicianId && !a.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Availability>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _dbSet
            .Include(a => a.Technician)
            .Where(a => a.StartTime >= from && a.EndTime <= to && !a.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Availability>> GetAvailableTechniciansInRangeAsync(DateTime from, DateTime to)
        => await _dbSet
            .Include(a => a.Technician)
            .Where(a => a.IsAvailable && a.StartTime >= from && a.EndTime <= to && !a.IsDeleted)
            .ToListAsync();
}
