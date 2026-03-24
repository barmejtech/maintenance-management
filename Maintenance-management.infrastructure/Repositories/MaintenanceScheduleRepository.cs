using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class MaintenanceScheduleRepository : Repository<MaintenanceSchedule>, IMaintenanceScheduleRepository
{
    public MaintenanceScheduleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MaintenanceSchedule>> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet
            .Include(s => s.Equipment)
            .Include(s => s.AssignedTechnician)
            .Include(s => s.AssignedGroup)
            .Where(s => s.EquipmentId == equipmentId && !s.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceSchedule>> GetActiveSchedulesAsync()
        => await _dbSet
            .Include(s => s.Equipment)
            .Include(s => s.AssignedTechnician)
            .Include(s => s.AssignedGroup)
            .Where(s => s.IsActive && !s.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceSchedule>> GetOverdueSchedulesAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(s => s.Equipment)
            .Include(s => s.AssignedTechnician)
            .Include(s => s.AssignedGroup)
            .Where(s => s.IsActive && !s.IsDeleted && s.NextDueAt.HasValue && s.NextDueAt.Value < now)
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceSchedule>> GetDueSoonAsync(int withinDays = 7)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(withinDays);
        return await _dbSet
            .Include(s => s.Equipment)
            .Include(s => s.AssignedTechnician)
            .Include(s => s.AssignedGroup)
            .Where(s => s.IsActive && !s.IsDeleted && s.NextDueAt.HasValue
                        && s.NextDueAt.Value >= now && s.NextDueAt.Value <= threshold)
            .ToListAsync();
    }
}
