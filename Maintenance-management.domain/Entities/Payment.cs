using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class Payment : BaseEntity
{
    public Guid PremiumMaintenanceRequestId { get; set; }
    public PremiumMaintenanceRequest? PremiumMaintenanceRequest { get; set; }

    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.OnlinePayment;
    public string? TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }
    public string? FailureReason { get; set; }
}
