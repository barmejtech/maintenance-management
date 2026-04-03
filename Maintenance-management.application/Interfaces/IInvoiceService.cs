using Maintenance_management.application.DTOs.Invoice;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(Guid id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto, string createdByUserId);
    Task<InvoiceDto?> UpdateAsync(Guid id, UpdateInvoiceDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<InvoiceDto>> GetByStatusAsync(InvoiceStatus status);
    Task<IEnumerable<InvoiceDto>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<InvoiceDto>> GetByMaintenanceReportIdAsync(Guid reportId);
}
