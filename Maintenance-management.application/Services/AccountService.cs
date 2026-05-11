using Maintenance_management.application.DTOs.Account;
using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo) => _repo = repo;

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithChildrenAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
    {
        if (await _repo.CodeExistsAsync(dto.AccountCode))
            throw new InvalidOperationException("Account code already exists.");

        var entity = new Account
        {
            AccountCode = dto.AccountCode,
            Name = dto.Name,
            Type = dto.Type,
            OpeningBalance = dto.OpeningBalance,
            OpeningBalanceDate = dto.OpeningBalanceDate,
            ParentAccountId = dto.ParentAccountId,
            Description = dto.Description,
            Balance = dto.OpeningBalance ?? 0,
            IsActive = true
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<AccountDto?> UpdateAsync(Guid id, UpdateAccountDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Name = dto.Name;
        item.IsActive = dto.IsActive;
        item.Description = dto.Description;
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

    public async Task<decimal> GetBalanceAsync(Guid id)
    {
        return await _repo.GetBalanceAsync(id);
    }

    private static AccountDto MapToDto(Account a) => new()
    {
        Id = a.Id,
        AccountCode = a.AccountCode,
        Name = a.Name,
        Type = a.Type,
        Balance = a.Balance,
        OpeningBalance = a.OpeningBalance,
        OpeningBalanceDate = a.OpeningBalanceDate,
        ParentAccountId = a.ParentAccountId,
        ParentAccountName = a.ParentAccount?.Name,
        IsActive = a.IsActive,
        Description = a.Description,
        ChildAccounts = a.ChildAccounts?.Where(c => !c.IsDeleted).Select(MapToDto).ToList() ?? new(),
        CreatedAt = a.CreatedAt
    };

    public Task<AccountProfileDto> GetProfileAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<AccountProfileDto> UpdateProfileAsync(string userId, UpdateAccountProfileDto dto)
    {
        throw new NotImplementedException();
    }

    public Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        throw new NotImplementedException();
    }
}