using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class AccountGLService : IAccountGLService
{
    private readonly IAccountRepository _repo;
    private readonly IJournalEntryRepository _journalRepo;

    public AccountGLService(IAccountRepository repo, IJournalEntryRepository journalRepo)
    {
        _repo = repo;
        _journalRepo = journalRepo;
    }

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
        => await _repo.GetBalanceAsync(id);

    public async Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate)
    {
        var accounts = await _repo.GetAllAsync();
        var activeAccounts = accounts.Where(a => !a.IsDeleted && a.IsActive).ToList();

        // Normal balance conventions:
        // Debit-normal: Asset (0), Expense (4)
        // Credit-normal: Liability (1), Equity (2), Revenue (3)
        var debitNormalTypes = new[] { AccountType.Asset, AccountType.Expense };

        var accountLines = activeAccounts.Select(a =>
        {
            bool isDebitNormal = debitNormalTypes.Contains(a.Type);
            return new TrialBalanceAccountDto
            {
                AccountCode = a.AccountCode,
                AccountName = a.Name,
                Debit = isDebitNormal && a.Balance >= 0 ? a.Balance
                      : !isDebitNormal && a.Balance < 0 ? Math.Abs(a.Balance)
                      : 0,
                Credit = !isDebitNormal && a.Balance >= 0 ? a.Balance
                       : isDebitNormal && a.Balance < 0 ? Math.Abs(a.Balance)
                       : 0
            };
        }).ToList();

        return new TrialBalanceDto
        {
            Accounts = accountLines,
            TotalDebit = accountLines.Sum(a => a.Debit),
            TotalCredit = accountLines.Sum(a => a.Credit)
        };
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
}
