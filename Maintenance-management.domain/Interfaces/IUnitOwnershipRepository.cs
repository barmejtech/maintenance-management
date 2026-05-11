using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IUnitOwnershipRepository : IRepository<UnitOwnership>
{
    Task<UnitOwnership?> GetActiveByUnitAsync(Guid unitId);
    Task<IEnumerable<UnitOwnership>> GetByUnitAsync(Guid unitId);
}
