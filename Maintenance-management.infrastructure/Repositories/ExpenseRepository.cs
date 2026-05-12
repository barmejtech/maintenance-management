using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class ExpenseRepository : Repository<Expense>, IExpenseRepository
{
    public ExpenseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Expense?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(e => e.Vendor)
            .Include(e => e.Renovation)
            .Include(e => e.SparePart)
            .Include(e => e.PaymentVouchers)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<IEnumerable<Expense>> GetAllWithDetailsAsync()
        => await _dbSet
            .Include(e => e.Vendor)
            .Include(e => e.Renovation)
            .Include(e => e.SparePart)
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();

    public async Task<IEnumerable<Expense>> GetByVendorIdAsync(Guid vendorId)
        => await _dbSet
            .Include(e => e.Vendor)
            .Where(e => e.VendorId == vendorId && !e.IsDeleted)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();

    public async Task<IEnumerable<Expense>> GetByStatusAsync(ExpenseStatus status)
        => await _dbSet
            .Include(e => e.Vendor)
            .Where(e => e.Status == status && !e.IsDeleted)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();

    public async Task<IEnumerable<Expense>> GetByRenovationIdAsync(Guid renovationId)
        => await _dbSet
            .Include(e => e.Vendor)
            .Where(e => e.RenovationId == renovationId && !e.IsDeleted)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
}
