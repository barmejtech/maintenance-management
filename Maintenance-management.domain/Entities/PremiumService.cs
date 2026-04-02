using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class PremiumService : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PremiumServiceType ServiceType { get; set; } = PremiumServiceType.Preventive;
    public decimal Price { get; set; }
    public int DurationHours { get; set; } = 1;
    // Reuses TaskPriority enum (Low/Medium/High/Critical) - same semantics for premium services
    public TaskPriority PriorityLevel { get; set; } = TaskPriority.Medium;
    public bool IsActive { get; set; } = true;
    public string? Features { get; set; }

    public ICollection<PremiumMaintenanceRequest> Requests { get; set; } = new List<PremiumMaintenanceRequest>();
}
