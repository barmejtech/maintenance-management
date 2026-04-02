using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Payment?> GetByRequestIdAsync(Guid premiumMaintenanceRequestId)
        => await _dbSet
            .FirstOrDefaultAsync(p => p.PremiumMaintenanceRequestId == premiumMaintenanceRequestId && !p.IsDeleted);
}
