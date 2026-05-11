using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class BankReconciliation : BaseEntity
{
    public string BankAccountName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public DateTime StatementDate { get; set; }
    public DateTime StatementStartDate { get; set; }
    public DateTime StatementEndDate { get; set; }

    public decimal StatementOpeningBalance { get; set; }
    public decimal StatementClosingBalance { get; set; }
    public decimal SystemOpeningBalance { get; set; }
    public decimal SystemClosingBalance { get; set; }

    public decimal Difference { get; set; }
    public bool IsReconciled { get; set; } = false;
    public DateTime? ReconciledAt { get; set; }
    public string? ReconciledByUserId { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public ICollection<ReconciliationEntry> Entries { get; set; } = new List<ReconciliationEntry>();
}

public class ReconciliationEntry : BaseEntity
{
    public Guid BankReconciliationId { get; set; }
    public BankReconciliation? BankReconciliation { get; set; }

    public string TransactionType { get; set; } = string.Empty;  // "Payment", "Deposit", "Fee"
    public Guid? TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsMatched { get; set; }
    public string? Notes { get; set; }
}