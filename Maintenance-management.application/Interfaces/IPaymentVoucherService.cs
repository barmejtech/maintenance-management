using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IPaymentVoucherService
{
    Task<IEnumerable<PaymentVoucherDto>> GetAllAsync();
    Task<PaymentVoucherDto?> GetByIdAsync(Guid id);
    Task<PaymentVoucherDto> CreateAsync(CreatePaymentVoucherDto dto, string createdByUserId);
    Task<PaymentVoucherDto?> UpdateAsync(Guid id, UpdatePaymentVoucherDto dto);
    Task<PaymentVoucherDto?> MarkAsPrintedAsync(Guid id, string printedByUserId);
    Task<bool> DeleteAsync(Guid id);
}