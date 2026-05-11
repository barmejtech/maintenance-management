using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<IEnumerable<Tenant>> GetByUnitIdAsync(Guid unitId);
}
