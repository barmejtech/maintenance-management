namespace Maintenance_management.domain.Entities;

public class JournalEntry : BaseEntity
{
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public bool IsPosted { get; set; } = true;
    public DateTime? PostedDate { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Source reference (e.g., Invoice ID, Payment ID)
    public string? SourceType { get; set; }  // "Invoice", "Payment", "Expense"
    public string? SourceId { get; set; }

    // Navigation
    public ICollection<JournalLineItem> LineItems { get; set; } = new List<JournalLineItem>();
}