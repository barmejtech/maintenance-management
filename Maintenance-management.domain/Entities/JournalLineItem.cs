using System.Security.Principal;

namespace Maintenance_management.domain.Entities;

public class JournalLineItem : BaseEntity
{
    public Guid JournalEntryId { get; set; }
    public JournalEntry? JournalEntry { get; set; }

    public Guid AccountId { get; set; }
    public Account? Account { get; set; }

    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Description { get; set; }

    // For audit trail
    public string? ReferenceType { get; set; }  // "Invoice", "Payment", "Expense"
    public string? ReferenceId { get; set; }
}