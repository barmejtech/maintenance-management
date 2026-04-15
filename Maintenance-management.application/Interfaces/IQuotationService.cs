using Maintenance_management.application.DTOs.Quotation;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IQuotationService
{
    Task<IEnumerable<QuotationDto>> GetAllAsync();
    Task<QuotationDto?> GetByIdAsync(Guid id);
    Task<QuotationDto> CreateAsync(CreateQuotationDto dto, string createdByUserId);
    Task<QuotationDto?> UpdateAsync(Guid id, UpdateQuotationDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<QuotationDto>> GetByStatusAsync(QuotationStatus status);
    Task<IEnumerable<QuotationDto>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<QuotationDto>> GetByMaintenanceRequestIdAsync(Guid requestId);
}
