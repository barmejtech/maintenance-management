using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IPremiumMaintenanceRequestRepository : IRepository<PremiumMaintenanceRequest>
{
    Task<PremiumMaintenanceRequest?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<PremiumMaintenanceRequest>> GetByClientIdAsync(Guid clientId);
}
