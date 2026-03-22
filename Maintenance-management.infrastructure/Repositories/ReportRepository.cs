using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class ReportRepository : Repository<MaintenanceReport>, IReportRepository
{
    public ReportRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MaintenanceReport>> GetByTaskOrderIdAsync(Guid taskOrderId)
        => await _dbSet
            .Include(r => r.TaskOrder)
            .Where(r => r.TaskOrderId == taskOrderId && !r.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceReport>> GetByTechnicianNameAsync(string name)
        => await _dbSet
            .Where(r => r.TechnicianName.Contains(name) && !r.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceReport>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _dbSet
            .Include(r => r.TaskOrder)
            .Where(r => r.ReportDate >= from && r.ReportDate <= to && !r.IsDeleted)
            .ToListAsync();
}
