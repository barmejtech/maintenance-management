using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IUnitRepository : IRepository<Unit>
{
    Task<Unit?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Unit>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<bool> UnitNumberExistsAsync(string unitNumber, Guid? excludeId = null);
}
