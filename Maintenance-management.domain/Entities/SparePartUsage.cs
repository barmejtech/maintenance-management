namespace Maintenance_management.domain.Entities;

public class SparePartUsage : BaseEntity
{
    public Guid SparePartId { get; set; }
    public SparePart SparePart { get; set; } = null!;

    public Guid? TaskOrderId { get; set; }
    public TaskOrder? TaskOrder { get; set; }

    public int QuantityUsed { get; set; }
    public string? Notes { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    public string UsedByUserId { get; set; } = string.Empty;
}
