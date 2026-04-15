using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class QuotationRepository : Repository<Quotation>, IQuotationRepository
{
    public QuotationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Quotation>> GetByStatusAsync(QuotationStatus status)
        => await _dbSet
            .Include(q => q.LineItems)
            .Where(q => q.Status == status && !q.IsDeleted)
            .ToListAsync();

    public async Task<Quotation?> GetWithLineItemsAsync(Guid id)
        => await _dbSet
            .Include(q => q.LineItems)
            .Include(q => q.MaintenanceRequest)
            .Include(q => q.Client)
            .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);

    public async Task<IEnumerable<Quotation>> GetByClientIdAsync(Guid clientId)
        => await _dbSet
            .Include(q => q.LineItems)
            .Include(q => q.Client)
            .Where(q => q.ClientId == clientId && !q.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Quotation>> GetByMaintenanceRequestIdAsync(Guid requestId)
        => await _dbSet
            .Include(q => q.LineItems)
            .Include(q => q.MaintenanceRequest)
            .Where(q => q.MaintenanceRequestId == requestId && !q.IsDeleted)
            .ToListAsync();
}
