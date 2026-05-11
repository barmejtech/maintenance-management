using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IUnitTypeRepository : IRepository<UnitType>
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}
