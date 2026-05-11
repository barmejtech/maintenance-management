using Maintenance_management.domain.Enums;
using System.Numerics;

namespace Maintenance_management.domain.Entities;

public enum ExpenseStatus
{
    Draft = 0,
    Approved = 1,
    Paid = 2,
    Cancelled = 3,
    Overdue = 4
}

public class Expense : BaseEntity
{
    public string ExpenseNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public decimal Amount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public ExpenseStatus Status { get; set; } = ExpenseStatus.Draft;

    public string? Description { get; set; }
    public string? InvoiceNumber { get; set; }  // Vendor invoice number
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? ApprovedByUserId { get; set; }

    // Links to other entities
    public Guid? RenovationId { get; set; }
    public Renovation? Renovation { get; set; }

    public Guid? SparePartId { get; set; }
    public SparePart? SparePart { get; set; }

    // Navigation
    public ICollection<Document> Attachments { get; set; } = new List<Document>();
    public ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();
    public JournalEntry? JournalEntry { get; set; }
}