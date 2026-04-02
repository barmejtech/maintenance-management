using Maintenance_management.application.DTOs.Payment;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IPremiumMaintenanceRequestRepository _requestRepo;

    public PaymentService(
        IPaymentRepository paymentRepo,
        IPremiumMaintenanceRequestRepository requestRepo)
    {
        _paymentRepo = paymentRepo;
        _requestRepo = requestRepo;
    }

    public async Task<PaymentDto?> GetByRequestIdAsync(Guid requestId)
    {
        var payment = await _paymentRepo.GetByRequestIdAsync(requestId);
        return payment is null ? null : MapToDto(payment);
    }

    public async Task<PaymentCheckoutDto> InitiatePaymentAsync(InitiatePaymentDto dto)
    {
        var request = await _requestRepo.GetWithDetailsAsync(dto.PremiumMaintenanceRequestId);
        if (request is null || request.IsDeleted)
            throw new InvalidOperationException("Premium maintenance request not found.");

        var amount = request.PremiumService?.Price ?? 0;

        // Check if a payment already exists for this request
        var existing = await _paymentRepo.GetByRequestIdAsync(dto.PremiumMaintenanceRequestId);
        Payment payment;

        if (existing is not null && !existing.IsDeleted)
        {
            existing.Status = PaymentStatus.Processing;
            existing.PaymentMethod = dto.PaymentMethod;
            existing.UpdatedAt = DateTime.UtcNow;
            await _paymentRepo.UpdateAsync(existing);
            payment = existing;
        }
        else
        {
            payment = new Payment
            {
                PremiumMaintenanceRequestId = dto.PremiumMaintenanceRequestId,
                Amount = amount,
                Status = PaymentStatus.Processing,
                PaymentMethod = dto.PaymentMethod
            };
            payment = await _paymentRepo.AddAsync(payment);
        }

        // Update the request status
        request.Status = PremiumMaintenanceStatus.PaymentPending;
        request.UpdatedAt = DateTime.UtcNow;
        await _requestRepo.UpdateAsync(request);

        // Generate a simulated checkout session (in production, this would call Stripe/PayPal API)
        var clientSecret = $"pi_{Guid.NewGuid():N}_secret_{Guid.NewGuid():N}";

        return new PaymentCheckoutDto
        {
            PaymentId = payment.Id,
            CheckoutUrl = $"/premium-maintenance/checkout/{payment.Id}",
            ClientSecret = clientSecret,
            Amount = amount,
            Currency = "USD"
        };
    }

    public async Task<PaymentDto?> ConfirmPaymentAsync(Guid paymentId, ConfirmPaymentDto dto)
    {
        var payment = await _paymentRepo.GetByIdAsync(paymentId);
        if (payment is null || payment.IsDeleted) return null;

        payment.Status = PaymentStatus.Completed;
        payment.TransactionId = dto.TransactionId;
        payment.PaymentDate = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;
        await _paymentRepo.UpdateAsync(payment);

        // Update the request status
        var request = await _requestRepo.GetByIdAsync(payment.PremiumMaintenanceRequestId);
        if (request is not null)
        {
            request.Status = PremiumMaintenanceStatus.Paid;
            request.UpdatedAt = DateTime.UtcNow;
            await _requestRepo.UpdateAsync(request);
        }

        return MapToDto(payment);
    }

    public async Task<PaymentDto?> CancelPaymentAsync(Guid paymentId)
    {
        var payment = await _paymentRepo.GetByIdAsync(paymentId);
        if (payment is null || payment.IsDeleted) return null;

        payment.Status = PaymentStatus.Cancelled;
        payment.UpdatedAt = DateTime.UtcNow;
        await _paymentRepo.UpdateAsync(payment);

        var request = await _requestRepo.GetByIdAsync(payment.PremiumMaintenanceRequestId);
        if (request is not null)
        {
            request.Status = PremiumMaintenanceStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;
            await _requestRepo.UpdateAsync(request);
        }

        return MapToDto(payment);
    }

    private static PaymentDto MapToDto(Payment p) => new()
    {
        Id = p.Id,
        PremiumMaintenanceRequestId = p.PremiumMaintenanceRequestId,
        Amount = p.Amount,
        Status = p.Status,
        PaymentMethod = p.PaymentMethod,
        TransactionId = p.TransactionId,
        PaymentDate = p.PaymentDate,
        Notes = p.Notes,
        FailureReason = p.FailureReason,
        CreatedAt = p.CreatedAt
    };
}
