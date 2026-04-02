using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class PremiumMaintenanceRequest : BaseEntity
{
    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    public Guid PremiumServiceId { get; set; }
    public PremiumService? PremiumService { get; set; }

    public PremiumMaintenanceStatus Status { get; set; } = PremiumMaintenanceStatus.Draft;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledDate { get; set; }
    public string? Notes { get; set; }
    public string? Address { get; set; }

    public Payment? Payment { get; set; }
}
