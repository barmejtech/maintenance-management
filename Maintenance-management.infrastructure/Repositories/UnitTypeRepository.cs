using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class UnitTypeRepository : Repository<UnitType>, IUnitTypeRepository
{
    public UnitTypeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
        => await _dbSet.AnyAsync(ut => !ut.IsDeleted
            && ut.Name == name
            && (!excludeId.HasValue || ut.Id != excludeId.Value));
}
