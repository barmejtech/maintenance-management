using Maintenance_management.application.DTOs.Payment;

namespace Maintenance_management.application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto?> GetByRequestIdAsync(Guid requestId);
    Task<PaymentCheckoutDto> InitiatePaymentAsync(InitiatePaymentDto dto);
    Task<PaymentDto?> ConfirmPaymentAsync(Guid paymentId, ConfirmPaymentDto dto);
    Task<PaymentDto?> CancelPaymentAsync(Guid paymentId);
}
