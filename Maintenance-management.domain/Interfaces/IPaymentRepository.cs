using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByRequestIdAsync(Guid premiumMaintenanceRequestId);
}
