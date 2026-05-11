namespace Maintenance_management.domain.Entities;

public class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }  // GST/SST/VAT
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<SparePart> SuppliedParts { get; set; } = new List<SparePart>();
}