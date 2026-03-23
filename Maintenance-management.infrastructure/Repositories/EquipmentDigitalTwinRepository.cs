using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class EquipmentDigitalTwinRepository : Repository<EquipmentDigitalTwin>, IEquipmentDigitalTwinRepository
{
    public EquipmentDigitalTwinRepository(ApplicationDbContext context) : base(context) { }

    public async Task<EquipmentDigitalTwin?> GetByEquipmentIdAsync(Guid equipmentId)
        => await _dbSet
            .Include(t => t.Equipment)
            .FirstOrDefaultAsync(t => t.EquipmentId == equipmentId && !t.IsDeleted);

    public async Task<IEnumerable<EquipmentDigitalTwin>> GetAllWithEquipmentAsync()
        => await _dbSet
            .Include(t => t.Equipment)
            .Where(t => !t.IsDeleted)
            .ToListAsync();
}
