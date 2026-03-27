using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetByStatusAsync(Enums.InvoiceStatus status);
    Task<Invoice?> GetByTaskOrderIdAsync(Guid taskOrderId);
    Task<Invoice?> GetWithLineItemsAsync(Guid id);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId);
}
