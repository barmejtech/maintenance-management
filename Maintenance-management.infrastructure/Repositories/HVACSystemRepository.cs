using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class HVACSystemRepository : Repository<HVACSystem>, IHVACSystemRepository
{
    public HVACSystemRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<HVACSystem>> GetDueForInspectionAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(h => !h.IsDeleted && h.NextInspectionDate.HasValue && h.NextInspectionDate.Value <= now.AddDays(7))
            .ToListAsync();
    }

    public async Task<HVACSystem?> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet.FirstOrDefaultAsync(h => h.EquipmentId == equipmentId && !h.IsDeleted);
}
