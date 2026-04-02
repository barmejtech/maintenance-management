using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.PremiumMaintenanceRequest;

public class PremiumMaintenanceRequestDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public Guid PremiumServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public PremiumServiceType ServiceType { get; set; }
    public PremiumMaintenanceStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public string? Address { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePremiumMaintenanceRequestDto
{
    public Guid ClientId { get; set; }
    public Guid PremiumServiceId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public string? Address { get; set; }
}

public class UpdatePremiumMaintenanceRequestDto
{
    public PremiumMaintenanceStatus Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public string? Address { get; set; }
}
