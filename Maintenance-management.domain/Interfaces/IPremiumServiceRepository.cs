using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IPremiumServiceRepository : IRepository<PremiumService>
{
    Task<IEnumerable<PremiumService>> GetActiveAsync();
}
