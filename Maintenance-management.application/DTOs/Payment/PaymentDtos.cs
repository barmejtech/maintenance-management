using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.Payment;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid PremiumMaintenanceRequestId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InitiatePaymentDto
{
    public Guid PremiumMaintenanceRequestId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.OnlinePayment;
}

public class ConfirmPaymentDto
{
    public string TransactionId { get; set; } = string.Empty;
}

public class PaymentCheckoutDto
{
    public Guid PaymentId { get; set; }
    public string CheckoutUrl { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}
