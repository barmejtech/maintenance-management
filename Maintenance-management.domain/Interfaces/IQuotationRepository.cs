using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IQuotationRepository : IRepository<Quotation>
{
    Task<IEnumerable<Quotation>> GetByStatusAsync(QuotationStatus status);
    Task<Quotation?> GetWithLineItemsAsync(Guid id);
    Task<IEnumerable<Quotation>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Quotation>> GetByMaintenanceRequestIdAsync(Guid requestId);
}
