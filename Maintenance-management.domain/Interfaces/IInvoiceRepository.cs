using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetByStatusAsync(Enums.InvoiceStatus status);
    Task<Invoice?> GetByTaskOrderIdAsync(Guid taskOrderId);
    Task<Invoice?> GetWithLineItemsAsync(Guid id);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Invoice>> GetByMaintenanceReportIdAsync(Guid reportId);
    Task<IEnumerable<Invoice>> GetByUnitIdAsync(Guid unitId);
    Task<IEnumerable<Invoice>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<Invoice>> GetByUnitOwnershipIdAsync(Guid unitOwnershipId);
    Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync(DateTime asOfDate);

}
