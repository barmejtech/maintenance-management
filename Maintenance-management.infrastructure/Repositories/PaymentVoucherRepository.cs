using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class PaymentVoucherRepository : Repository<PaymentVoucher>, IPaymentVoucherRepository
{
    public PaymentVoucherRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PaymentVoucher?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(p => p.Expense)
                .ThenInclude(e => e!.Vendor)
            .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

    public async Task<IEnumerable<PaymentVoucher>> GetAllWithDetailsAsync()
        => await _dbSet
            .Include(p => p.Expense)
                .ThenInclude(e => e!.Vendor)
            .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
            .Include(p => p.Owner)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.VoucherDate)
            .ToListAsync();

    public async Task<IEnumerable<PaymentVoucher>> GetByExpenseIdAsync(Guid expenseId)
        => await _dbSet
            .Include(p => p.Expense)
                .ThenInclude(e => e!.Vendor)
            .Where(p => p.ExpenseId == expenseId && !p.IsDeleted)
            .OrderByDescending(p => p.VoucherDate)
            .ToListAsync();

    public async Task<IEnumerable<PaymentVoucher>> GetByInvoiceIdAsync(Guid invoiceId)
        => await _dbSet
            .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
            .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
            .OrderByDescending(p => p.VoucherDate)
            .ToListAsync();
}
