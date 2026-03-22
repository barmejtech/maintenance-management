using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Equipment>> GetByStatusAsync(EquipmentStatus status)
        => await _dbSet.Where(e => e.Status == status && !e.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Equipment>> GetDueForMaintenanceAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(e => !e.IsDeleted && e.NextMaintenanceDate.HasValue && e.NextMaintenanceDate.Value <= now.AddDays(7))
            .ToListAsync();
    }
}
