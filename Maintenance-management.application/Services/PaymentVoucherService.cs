using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class PaymentVoucherService : IPaymentVoucherService
{
    private readonly IPaymentVoucherRepository _repo;
    private readonly IExpenseRepository _expenseRepo;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IOwnerRepository _ownerRepo;

    public PaymentVoucherService(
        IPaymentVoucherRepository repo,
        IExpenseRepository expenseRepo,
        IInvoiceRepository invoiceRepo,
        IOwnerRepository ownerRepo)
    {
        _repo = repo;
        _expenseRepo = expenseRepo;
        _invoiceRepo = invoiceRepo;
        _ownerRepo = ownerRepo;
    }

    public async Task<IEnumerable<PaymentVoucherDto>> GetAllAsync()
    {
        var items = await _repo.GetAllWithDetailsAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<PaymentVoucherDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<PaymentVoucherDto> CreateAsync(CreatePaymentVoucherDto dto, string createdByUserId)
    {
        // Validate linked entity
        if (dto.ExpenseId.HasValue && !await _expenseRepo.ExistsAsync(dto.ExpenseId.Value))
            throw new KeyNotFoundException("Expense not found.");

        if (dto.InvoiceId.HasValue && !await _invoiceRepo.ExistsAsync(dto.InvoiceId.Value))
            throw new KeyNotFoundException("Invoice not found.");

        if (dto.OwnerId.HasValue && !await _ownerRepo.ExistsAsync(dto.OwnerId.Value))
            throw new KeyNotFoundException("Owner not found.");

        var entity = new PaymentVoucher
        {
            VoucherNumber = GenerateVoucherNumber(),
            VoucherDate = DateTime.UtcNow,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            ChequeNumber = dto.ChequeNumber,
            BankName = dto.BankName,
            ChequeDate = dto.ChequeDate,
            ExpenseId = dto.ExpenseId,
            InvoiceId = dto.InvoiceId,
            OwnerId = dto.OwnerId,
            PayeeName = dto.PayeeName,
            Description = dto.Description,
            CreatedByUserId = createdByUserId,
            IsPrinted = false
        };

        var created = await _repo.AddAsync(entity);

        // Update linked expense status if needed
        if (dto.ExpenseId.HasValue)
        {
            var expense = await _expenseRepo.GetByIdAsync(dto.ExpenseId.Value);
            if (expense != null && expense.Status != ExpenseStatus.Paid)
            {
                expense.Status = ExpenseStatus.Paid;
                expense.PaidDate = DateTime.UtcNow;
                await _expenseRepo.UpdateAsync(expense);
            }
        }

        // Update linked invoice status if needed
        if (dto.InvoiceId.HasValue)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId.Value);
            if (invoice != null && invoice.Status != InvoiceStatus.Paid)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidDate = DateTime.UtcNow;
                await _invoiceRepo.UpdateAsync(invoice);
            }
        }

        return MapToDto(created);
    }

    public async Task<PaymentVoucherDto?> UpdateAsync(Guid id, UpdatePaymentVoucherDto dto)
    {
        var item = await _repo.GetWithDetailsAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.PaymentMethod = dto.PaymentMethod;
        item.ChequeNumber = dto.ChequeNumber;
        item.BankName = dto.BankName;
        item.ChequeDate = dto.ChequeDate;
        item.Description = dto.Description;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<PaymentVoucherDto?> MarkAsPrintedAsync(Guid id, string printedByUserId)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.IsPrinted = true;
        item.PrintedAt = DateTime.UtcNow;
        item.PrintedByUserId = printedByUserId;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return false;

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static string GenerateVoucherNumber()
        => $"PV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    private static PaymentVoucherDto MapToDto(PaymentVoucher pv) => new()
    {
        Id = pv.Id,
        VoucherNumber = pv.VoucherNumber,
        VoucherDate = pv.VoucherDate,
        Amount = pv.Amount,
        PaymentMethod = pv.PaymentMethod,
        ChequeNumber = pv.ChequeNumber,
        BankName = pv.BankName,
        ChequeDate = pv.ChequeDate,
        ExpenseId = pv.ExpenseId,
        ExpenseNumber = pv.Expense?.ExpenseNumber,
        VendorName = pv.Expense?.Vendor?.Name,
        InvoiceId = pv.InvoiceId,
        InvoiceNumber = pv.Invoice?.InvoiceNumber,
        ClientName = pv.Invoice?.ClientName,
        OwnerId = pv.OwnerId,
        OwnerName = pv.Owner?.FullName,
        PayeeName = pv.PayeeName,
        Description = pv.Description,
        IsPrinted = pv.IsPrinted,
        PrintedAt = pv.PrintedAt,
        PrintedByUserId = pv.PrintedByUserId,
        CreatedAt = pv.CreatedAt
    };
}