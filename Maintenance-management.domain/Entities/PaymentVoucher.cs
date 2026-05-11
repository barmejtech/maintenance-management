using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public class PaymentVoucher : BaseEntity
{
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    // For cheque printing
    public string? ChequeNumber { get; set; }
    public string? BankName { get; set; }
    public string? ChequeDate { get; set; }

    // Payment to vendor (Expense)
    public Guid? ExpenseId { get; set; }
    public Expense? Expense { get; set; }

    // Payment from customer (Invoice)
    public Guid? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    // Payment to owner (refund, dividend)
    public Guid? OwnerId { get; set; }
    public Owner? Owner { get; set; }

    public string? PayeeName { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? ApprovedByUserId { get; set; }

    // Print tracking
    public bool IsPrinted { get; set; } = false;
    public DateTime? PrintedAt { get; set; }
    public string? PrintedByUserId { get; set; }

    // Navigation
    public ICollection<Document> Attachments { get; set; } = new List<Document>();
}