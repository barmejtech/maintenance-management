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
            .Include(i => i.Client)
            .Include(i => i.MaintenanceReport)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId)
        => await _dbSet
            .Include(i => i.LineItems)
            .Include(i => i.Client)
            .Where(i => i.ClientId == clientId && !i.IsDeleted)
            .ToListAsync();

    public async Task<IEnumerable<Invoice>> GetByMaintenanceReportIdAsync(Guid reportId)
        => await _dbSet
            .Include(i => i.LineItems)
            .Include(i => i.MaintenanceReport)
            .Where(i => i.MaintenanceReportId == reportId && !i.IsDeleted)
            .ToListAsync();


    public async Task<IEnumerable<Invoice>> GetByUnitIdAsync(Guid unitId)
    {
        return await _context.Invoices
            .Where(i => i.UnitId == unitId && !i.IsDeleted)
            .Include(i => i.Client)
            .Include(i => i.TaskOrder)
            .Include(i => i.MaintenanceReport)
            .Include(i => i.LineItems)
            .Include(i => i.Unit)
            .Include(i => i.Tenant)
            .Include(i => i.UnitOwnership)
                .ThenInclude(uo => uo.Owner)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByTenantIdAsync(Guid tenantId)
    {
        return await _context.Invoices
            .Where(i => i.TenantId == tenantId && !i.IsDeleted)
            .Include(i => i.Client)
            .Include(i => i.TaskOrder)
            .Include(i => i.MaintenanceReport)
            .Include(i => i.LineItems)
            .Include(i => i.Unit)
            .Include(i => i.Tenant)
            .Include(i => i.UnitOwnership)
                .ThenInclude(uo => uo.Owner)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByUnitOwnershipIdAsync(Guid unitOwnershipId)
    {
        return await _context.Invoices
            .Where(i => i.UnitOwnershipId == unitOwnershipId && !i.IsDeleted)
            .Include(i => i.Client)
            .Include(i => i.TaskOrder)
            .Include(i => i.MaintenanceReport)
            .Include(i => i.LineItems)
            .Include(i => i.Unit)
            .Include(i => i.Tenant)
            .Include(i => i.UnitOwnership)
                .ThenInclude(uo => uo.Owner)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync(DateTime asOfDate)
    {
        return await _context.Invoices
            .Where(i => i.Status != InvoiceStatus.Paid
                     && i.Status != InvoiceStatus.Cancelled
                     && i.DueDate.HasValue
                     && i.DueDate.Value <= asOfDate
                     && !i.IsDeleted)
            .Include(i => i.Client)
            .Include(i => i.Unit)
            .Include(i => i.Tenant)
            .Include(i => i.UnitOwnership)
                .ThenInclude(uo => uo!.Owner)
            .Include(i => i.LineItems)
            .OrderBy(i => i.DueDate)
            .ToListAsync();
    }
}
