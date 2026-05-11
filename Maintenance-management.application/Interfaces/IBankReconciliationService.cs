using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IBankReconciliationService
{
    Task<IEnumerable<BankReconciliationDto>> GetAllAsync();
    Task<BankReconciliationDto?> GetByIdAsync(Guid id);
    Task<BankReconciliationDto> CreateAsync(CreateBankReconciliationDto dto, string createdByUserId);
    Task<BankReconciliationDto?> ReconcileAsync(Guid id, CompleteReconciliationDto dto, string reconciledByUserId);
    Task<bool> DeleteAsync(Guid id);
}