using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAllAsync();
    Task<ExpenseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ExpenseDto>> GetByVendorIdAsync(Guid vendorId);
    Task<IEnumerable<ExpenseDto>> GetByStatusAsync(ExpenseStatus status);
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, string createdByUserId);
    Task<ExpenseDto?> UpdateAsync(Guid id, UpdateExpenseDto dto);
    Task<ExpenseDto?> ApproveAsync(Guid id, string approvedByUserId);
    Task<bool> DeleteAsync(Guid id);
}