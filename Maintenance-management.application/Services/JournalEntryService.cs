using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class JournalEntryService : IJournalEntryService
{
    private readonly IJournalEntryRepository _repo;
    private readonly IAccountRepository _accountRepo;

    public JournalEntryService(IJournalEntryRepository repo, IAccountRepository accountRepo)
    {
        _repo = repo;
        _accountRepo = accountRepo;
    }

    public async Task<IEnumerable<JournalEntryDto>> GetAllAsync()
    {
        var items = await _repo.GetAllWithLineItemsAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<JournalEntryDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string createdByUserId)
    {
        // Validate debit = credit
        var totalDebit = dto.LineItems.Sum(x => x.Debit);
        var totalCredit = dto.LineItems.Sum(x => x.Credit);

        if (totalDebit != totalCredit)
            throw new InvalidOperationException($"Debit ({totalDebit}) must equal Credit ({totalCredit})");

        // Validate accounts exist
        foreach (var line in dto.LineItems)
        {
            if (!await _accountRepo.ExistsAsync(line.AccountId))
                throw new KeyNotFoundException($"Account {line.AccountId} not found.");
        }

        var entity = new JournalEntry
        {
            EntryNumber = GenerateEntryNumber(),
            EntryDate = dto.EntryDate,
            Description = dto.Description,
            IsPosted = false,
            CreatedByUserId = createdByUserId,
            LineItems = dto.LineItems.Select(li => new JournalLineItem
            {
                AccountId = li.AccountId,
                Debit = li.Debit,
                Credit = li.Credit,
                Description = li.Description
            }).ToList()
        };

        var created = await _repo.AddAsync(entity);
        var withDetails = await _repo.GetWithLineItemsAsync(created.Id);
        return MapToDto(withDetails ?? created);
    }

    public async Task<JournalEntryDto?> UpdateAsync(Guid id, CreateJournalEntryDto dto)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (item.IsPosted)
            throw new InvalidOperationException("Cannot update a posted journal entry.");

        var totalDebit = dto.LineItems.Sum(x => x.Debit);
        var totalCredit = dto.LineItems.Sum(x => x.Credit);

        if (totalDebit != totalCredit)
            throw new InvalidOperationException($"Debit ({totalDebit}) must equal Credit ({totalCredit})");

        item.EntryDate = dto.EntryDate;
        item.Description = dto.Description;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<JournalEntryDto?> PostAsync(Guid id, string approvedByUserId)
    {
        var item = await _repo.GetWithLineItemsAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (item.IsPosted)
            throw new InvalidOperationException("Journal entry already posted.");

        // Update account balances
        foreach (var line in item.LineItems)
        {
            var account = await _accountRepo.GetByIdAsync(line.AccountId);
            if (account != null)
            {
                if (line.Debit > 0)
                    account.Balance += line.Debit;
                if (line.Credit > 0)
                    account.Balance -= line.Credit;

                account.UpdatedAt = DateTime.UtcNow;
                await _accountRepo.UpdateAsync(account);
            }
        }

        item.IsPosted = true;
        item.PostedDate = DateTime.UtcNow;
        item.ApprovedByUserId = approvedByUserId;
        item.ApprovedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return false;

        if (item.IsPosted)
            throw new InvalidOperationException("Cannot delete a posted journal entry.");

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static string GenerateEntryNumber()
        => $"JE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

    private static JournalEntryDto MapToDto(JournalEntry je) => new()
    {
        Id = je.Id,
        EntryNumber = je.EntryNumber,
        EntryDate = je.EntryDate,
        Description = je.Description,
        IsPosted = je.IsPosted,
        PostedDate = je.PostedDate,
        CreatedByUserId = je.CreatedByUserId,
        ApprovedByUserId = je.ApprovedByUserId,
        ApprovedAt = je.ApprovedAt,
        LineItems = je.LineItems.Select(li => new JournalLineItemDto
        {
            Id = li.Id,
            AccountId = li.AccountId,
            AccountCode = li.Account?.AccountCode ?? string.Empty,
            AccountName = li.Account?.Name ?? string.Empty,
            Debit = li.Debit,
            Credit = li.Credit,
            Description = li.Description
        }).ToList(),
        CreatedAt = je.CreatedAt
    };
}