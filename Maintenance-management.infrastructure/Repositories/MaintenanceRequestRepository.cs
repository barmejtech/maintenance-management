using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class MaintenanceRequestRepository : Repository<MaintenanceRequest>, IMaintenanceRequestRepository
{
    public MaintenanceRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MaintenanceRequest>> GetByClientIdAsync(Guid clientId)
        => await _dbSet
            .Include(r => r.Client)
            .Include(r => r.TaskOrder)
            .Include(r => r.Invoice)
            .Include(r => r.Assignments).ThenInclude(a => a.Technician)
            .Where(r => r.ClientId == clientId && !r.IsDeleted)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceRequest>> GetByStatusAsync(MaintenanceRequestStatus status)
        => await _dbSet
            .Include(r => r.Client)
            .Include(r => r.TaskOrder)
            .Include(r => r.Invoice)
            .Include(r => r.Assignments).ThenInclude(a => a.Technician)
            .Where(r => r.Status == status && !r.IsDeleted)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

    public async Task<MaintenanceRequest?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(r => r.Client)
            .Include(r => r.TaskOrder)
            .Include(r => r.Invoice)
            .Include(r => r.Assignments).ThenInclude(a => a.Technician)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

    public override async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
        => await _dbSet
            .Include(r => r.Client)
            .Include(r => r.TaskOrder)
            .Include(r => r.Invoice)
            .Include(r => r.Assignments).ThenInclude(a => a.Technician)
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

    public async Task AddAssignmentAsync(MaintenanceRequestAssignment assignment)
    {
        await _context.Set<MaintenanceRequestAssignment>().AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAssignmentsAsync(Guid requestId)
    {
        var existing = _context.Set<MaintenanceRequestAssignment>()
            .Where(a => a.MaintenanceRequestId == requestId);
        _context.Set<MaintenanceRequestAssignment>().RemoveRange(existing);
        await _context.SaveChangesAsync();
    }
}
