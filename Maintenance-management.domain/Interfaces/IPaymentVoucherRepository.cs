using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IPaymentVoucherRepository : IRepository<PaymentVoucher>
{
    Task<PaymentVoucher?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<PaymentVoucher>> GetAllWithDetailsAsync();
    Task<IEnumerable<PaymentVoucher>> GetByExpenseIdAsync(Guid expenseId);
    Task<IEnumerable<PaymentVoucher>> GetByInvoiceIdAsync(Guid invoiceId);
}