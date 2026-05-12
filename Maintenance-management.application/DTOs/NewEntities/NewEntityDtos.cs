using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using MeterType = Maintenance_management.domain.Enums.MeterType;

namespace Maintenance_management.application.DTOs.NewEntities;

// ==================== OWNER ====================

public class OwnerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UnitOwnershipSummaryDto> OwnedUnits { get; set; } = new();
}

public class UnitOwnershipSummaryDto
{
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public decimal OwnershipPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateOwnerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class UpdateOwnerDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

// ==================== UNIT OWNERSHIP ====================

public class UnitOwnershipDto
{
    public Guid Id { get; set; }
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal OwnershipPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUnitOwnershipDto
{
    public Guid UnitId { get; set; }
    public Guid OwnerId { get; set; }
    public decimal OwnershipPercentage { get; set; } = 100m;
    public DateTime PurchaseDate { get; set; }
}

public class UpdateUnitOwnershipDto
{
    public decimal OwnershipPercentage { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; }
}

public class TransferOwnershipDto
{
    public Guid NewOwnerId { get; set; }
    public DateTime TransferDate { get; set; }
    public decimal OwnershipPercentage { get; set; } = 100m;
}

// ==================== METER READING ====================

public class MeterReadingDto
{
    public Guid Id { get; set; }
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public Guid? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public domain.Enums.MeterType Type { get; set; }
    public double ReadingValue { get; set; }
    public double? PreviousReadingValue { get; set; }
    public double? Consumption { get; set; }
    public DateTime ReadingDate { get; set; }
    public string? PhotoUrl { get; set; }
    public string ReadByUserId { get; set; } = string.Empty;
    public string? ReadByName { get; set; }
    public string? Notes { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? CalculatedAmount { get; set; }
    public Guid? GeneratedInvoiceId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMeterReadingDto
{
    public Guid UnitId { get; set; }
    public Guid? EquipmentId { get; set; }
    public MeterType Type { get; set; }
    public double ReadingValue { get; set; }
    public DateTime ReadingDate { get; set; } = DateTime.UtcNow;
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
    public decimal? UnitPrice { get; set; }
}

public class UpdateMeterReadingDto
{
    public double ReadingValue { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }
}

public class BulkMeterReadingDto
{
    public MeterType Type { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal? UnitPrice { get; set; }
    public List<UnitMeterReadingDto> Readings { get; set; } = new();
}

public class UnitMeterReadingDto
{
    public Guid UnitId { get; set; }
    public double ReadingValue { get; set; }
}

public class MeterReadingChartDataDto
{
    public string Label { get; set; } = string.Empty;
    public double ReadingValue { get; set; }
    public double Consumption { get; set; }
    public decimal? Amount { get; set; }
}

// ==================== RENOVATION ====================

public class RenovationDto
{
    public Guid Id { get; set; }
    public Guid UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public domain.Entities.RenovationStatus Status { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
    public List<RenovationDocumentDto> Documents { get; set; } = new();
    public List<RenovationExpenseDto> Expenses { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class RenovationDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}

public class RenovationExpenseDto
{
    public Guid Id { get; set; }
    public string ExpenseNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string VendorName { get; set; } = string.Empty;
}

public class CreateRenovationDto
{
    public Guid UnitId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRenovationDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public domain.Entities.RenovationStatus Status { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorPhone { get; set; }
    public string? Notes { get; set; }
}

// ==================== ACCOUNT (GL) ====================

public class AccountDto
{
    public Guid Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public domain.Entities.AccountType Type { get; set; }
    public decimal Balance { get; set; }
    public decimal? OpeningBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }
    public Guid? ParentAccountId { get; set; }
    public string? ParentAccountName { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public List<AccountDto> ChildAccounts { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateAccountDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public domain.Entities.AccountType Type { get; set; }
    public decimal? OpeningBalance { get; set; }
    public DateTime? OpeningBalanceDate { get; set; }
    public Guid? ParentAccountId { get; set; }
    public string? Description { get; set; }
}

public class UpdateAccountDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class AccountBalanceDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
    public decimal NetBalance { get; set; }
}

// ==================== JOURNAL ENTRY ====================

public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public bool IsPosted { get; set; }
    public DateTime? PostedDate { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? CreatedByName { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? SourceType { get; set; }
    public string? SourceId { get; set; }
    public List<JournalLineItemDto> LineItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class JournalLineItemDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Description { get; set; }
}

public class CreateJournalEntryDto
{
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public List<CreateJournalLineItemDto> LineItems { get; set; } = new();
}

public class CreateJournalLineItemDto
{
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? Description { get; set; }
}

public class PostJournalEntryDto
{
    public string? ApprovedByUserId { get; set; }
}

// ==================== VENDOR ====================

public class VendorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalPurchased { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVendorDto
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? Notes { get; set; }
}

public class UpdateVendorDto
{
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

// ==================== EXPENSE ====================

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string ExpenseNumber { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public ExpenseStatus Status { get; set; }
    public string? Description { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid? RenovationId { get; set; }
    public string? RenovationTitle { get; set; }
    public Guid? SparePartId { get; set; }
    public string? SparePartName { get; set; }
    public List<PaymentVoucherSummaryDto> PaymentVouchers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateExpenseDto
{
    public Guid VendorId { get; set; }
    public decimal Amount { get; set; }
    public decimal? TaxAmount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Description { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid? RenovationId { get; set; }
    public Guid? SparePartId { get; set; }
}

public class UpdateExpenseDto
{
    public decimal Amount { get; set; }
    public decimal? TaxAmount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime? DueDate { get; set; }
    public ExpenseStatus Status { get; set; }
    public string? Description { get; set; }
    public string? InvoiceNumber { get; set; }
}

public class ApproveExpenseDto
{
    public string? ApprovedByUserId { get; set; }
}

// ==================== PAYMENT VOUCHER ====================

public class PaymentVoucherDto
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ChequeNumber { get; set; }
    public string? BankName { get; set; }
    public string? ChequeDate { get; set; }
    public Guid? ExpenseId { get; set; }
    public string? ExpenseNumber { get; set; }
    public string? VendorName { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? ClientName { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? PayeeName { get; set; }
    public string? Description { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime? PrintedAt { get; set; }
    public string? PrintedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentVoucherDto
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ChequeNumber { get; set; }
    public string? BankName { get; set; }
    public string? ChequeDate { get; set; }
    public Guid? ExpenseId { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? OwnerId { get; set; }
    public string? PayeeName { get; set; }
    public string? Description { get; set; }
}

public class UpdatePaymentVoucherDto
{
    public PaymentMethod PaymentMethod { get; set; }
    public string? ChequeNumber { get; set; }
    public string? BankName { get; set; }
    public string? ChequeDate { get; set; }
    public string? Description { get; set; }
}

public class PrintPaymentVoucherDto
{
    public bool IsPrinted { get; set; }
    public DateTime? PrintedAt { get; set; }
    public string? PrintedByUserId { get; set; }
}

public class PaymentVoucherSummaryDto
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime VoucherDate { get; set; }
}

// ==================== BANK RECONCILIATION ====================

public class BankReconciliationDto
{
    public Guid Id { get; set; }
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
    public bool IsReconciled { get; set; }
    public DateTime? ReconciledAt { get; set; }
    public string? ReconciledByUserId { get; set; }
    public string? Notes { get; set; }
    public List<ReconciliationEntryDto> Entries { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ReconciliationEntryDto
{
    public Guid Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public Guid? TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsMatched { get; set; }
    public string? Notes { get; set; }
}

public class CreateBankReconciliationDto
{
    public string BankAccountName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public DateTime StatementDate { get; set; }
    public DateTime StatementStartDate { get; set; }
    public DateTime StatementEndDate { get; set; }
    public decimal StatementOpeningBalance { get; set; }
    public decimal StatementClosingBalance { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBankReconciliationDto
{
    public decimal SystemOpeningBalance { get; set; }
    public decimal SystemClosingBalance { get; set; }
    public List<MatchReconciliationEntryDto> MatchedEntries { get; set; } = new();
}

public class MatchReconciliationEntryDto
{
    public Guid EntryId { get; set; }
    public bool IsMatched { get; set; }
    public Guid? MatchedTransactionId { get; set; }
}

public class CompleteReconciliationDto
{
    public bool IsReconciled { get; set; } = true;
    public string? Notes { get; set; }
}

// ==================== FINANCIAL REPORTS ====================

public class TrialBalanceDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public List<TrialBalanceAccountDto> Accounts { get; set; } = new();
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }

}

public class TrialBalanceAccountDto
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class ProfitLossDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public List<AccountBalanceDto> RevenueAccounts { get; set; } = new();
    public List<AccountBalanceDto> ExpenseAccounts { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class BalanceSheetDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public List<AccountBalanceDto> Assets { get; set; } = new();
    public List<AccountBalanceDto> Liabilities { get; set; } = new();
    public List<AccountBalanceDto> Equity { get; set; } = new();
    public DateTime AsOfDate { get; set; }
}

public class CashFlowDto
{
    public decimal OperatingCashFlow { get; set; }
    public decimal InvestingCashFlow { get; set; }
    public decimal FinancingCashFlow { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal BeginningCashBalance { get; set; }
    public decimal EndingCashBalance { get; set; }
    public List<CashFlowItemDto> OperatingItems { get; set; } = new();
    public List<CashFlowItemDto> InvestingItems { get; set; } = new();
    public List<CashFlowItemDto> FinancingItems { get; set; } = new();
}

public class CashFlowItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

public class AgingReportDto
{
    public decimal Current { get; set; }
    public decimal Days1To30 { get; set; }
    public decimal Days31To60 { get; set; }
    public decimal Days61To90 { get; set; }
    public decimal Days90Plus { get; set; }
    public decimal Total { get; set; }

    public List<AgingItemDto> CurrentItems { get; set; } = new();
    public List<AgingItemDto> Days1To30Items { get; set; } = new();
    public List<AgingItemDto> Days31To60Items { get; set; } = new();
    public List<AgingItemDto> Days61To90Items { get; set; } = new();
    public List<AgingItemDto> Days90PlusItems { get; set; } = new();
    public List<AgingItemDto> Items { get; set; } = new();
}

public class AgingItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public int DaysOverdue { get; set; }
    public string AgingBucket { get; set; } = string.Empty;
}

public class BulkMeterReadingResultDto
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<MeterReadingDto> Readings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}