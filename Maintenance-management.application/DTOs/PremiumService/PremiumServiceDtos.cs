using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.PremiumService;

public class PremiumServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PremiumServiceType ServiceType { get; set; }
    public decimal Price { get; set; }
    public int DurationHours { get; set; }
    public TaskPriority PriorityLevel { get; set; }
    public bool IsActive { get; set; }
    public string? Features { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePremiumServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PremiumServiceType ServiceType { get; set; } = PremiumServiceType.Preventive;
    public decimal Price { get; set; }
    public int DurationHours { get; set; } = 1;
    public TaskPriority PriorityLevel { get; set; } = TaskPriority.Medium;
    public bool IsActive { get; set; } = true;
    public string? Features { get; set; }
}

public class UpdatePremiumServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PremiumServiceType ServiceType { get; set; }
    public decimal Price { get; set; }
    public int DurationHours { get; set; }
    public TaskPriority PriorityLevel { get; set; }
    public bool IsActive { get; set; }
    public string? Features { get; set; }
}
