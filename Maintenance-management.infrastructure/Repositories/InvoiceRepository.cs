using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
        => await _dbSet
            .Include(i => i.LineItems)
            .Where(i => i.Status == status && !i.IsDeleted)
            .ToListAsync();

    public async Task<Invoice?> GetByTaskOrderIdAsync(Guid taskOrderId)
        => await _dbSet
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.TaskOrderId == taskOrderId && !i.IsDeleted);

    public async Task<Invoice?> GetWithLineItemsAsync(Guid id)
        => await _dbSet
            .Include(i => i.LineItems)
            .Include(i => i.TaskOrder)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
}
