using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IAccountGLService
{
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto?> GetByIdAsync(Guid id);
    Task<AccountDto> CreateAsync(CreateAccountDto dto);
    Task<AccountDto?> UpdateAsync(Guid id, UpdateAccountDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetBalanceAsync(Guid id);
    Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate);
}