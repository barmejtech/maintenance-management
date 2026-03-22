using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class TaskOrderRepository : Repository<TaskOrder>, ITaskOrderRepository
{
    public TaskOrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskOrder>> GetByTechnicianIdAsync(Guid technicianId)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Group)
            .Include(t => t.Equipment)
            .Where(t => t.TechnicianId == technicianId && !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<TaskOrder>> GetByGroupIdAsync(Guid groupId)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Group)
            .Include(t => t.Equipment)
            .Where(t => t.GroupId == groupId && !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<TaskOrder>> GetByStatusAsync(domain.Enums.TaskStatus status)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Group)
            .Include(t => t.Equipment)
            .Where(t => t.Status == status && !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<TaskOrder>> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Equipment)
            .Where(t => t.EquipmentId == equipmentId && !t.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<TaskOrder>> GetScheduledBetweenAsync(DateTime from, DateTime to)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Group)
            .Include(t => t.Equipment)
            .Where(t => t.ScheduledDate >= from && t.ScheduledDate <= to && !t.IsDeleted)
            .ToListAsync();

    public async Task<TaskOrder?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(t => t.Technician)
            .Include(t => t.Group)
            .Include(t => t.Equipment)
            .Include(t => t.Reports)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
}
