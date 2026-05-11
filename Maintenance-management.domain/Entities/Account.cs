using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public enum AccountType
{
    Asset = 0,
    Liability = 1,
    Equity = 2,
    Revenue = 3,
    Expense = 4
}

public class Account : BaseEntity
{
    public string AccountCode { get; set; } = string.Empty;  // "1000", "2000"
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public decimal? OpeningBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }

    // Hierarchical structure
    public Guid? ParentAccountId { get; set; }
    public Account? ParentAccount { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Account> ChildAccounts { get; set; } = new List<Account>();
    public ICollection<JournalLineItem> JournalLineItems { get; set; } = new List<JournalLineItem>();
}