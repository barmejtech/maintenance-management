using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public enum RenovationStatus
{
    Planned = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    OnHold = 4
}

public class Renovation : BaseEntity
{
    public Guid UnitId { get; set; }
    public Unit? Unit { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RenovationStatus Status { get; set; } = RenovationStatus.Planned;

    // Financial tracking
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }

    // Tracking
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }

    // Relationships
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<TaskOrder> RelatedTaskOrders { get; set; } = new List<TaskOrder>();
}