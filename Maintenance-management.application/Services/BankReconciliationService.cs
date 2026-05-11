using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class BankReconciliationService : IBankReconciliationService
{
    private readonly IBankReconciliationRepository _repo;
    private readonly IJournalEntryRepository _journalRepo;

    public BankReconciliationService(IBankReconciliationRepository repo, IJournalEntryRepository journalRepo)
    {
        _repo = repo;
        _journalRepo = journalRepo;
    }

    public async Task<IEnumerable<BankReconciliationDto>> GetAllAsync()
    {
        var items = await _repo.GetAllWithEntriesAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<BankReconciliationDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithEntriesAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<BankReconciliationDto> CreateAsync(CreateBankReconciliationDto dto, string createdByUserId)
    {
        var entity = new BankReconciliation
        {
            BankAccountName = dto.BankAccountName,
            BankAccountNumber = dto.BankAccountNumber,
            StatementDate = dto.StatementDate,
            StatementStartDate = dto.StatementStartDate,
            StatementEndDate = dto.StatementEndDate,
            StatementOpeningBalance = dto.StatementOpeningBalance,
            StatementClosingBalance = dto.StatementClosingBalance,
            Notes = dto.Notes,
            IsReconciled = false
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<BankReconciliationDto?> AddEntriesAsync(Guid id, List<ReconciliationEntry> entries)
    {
        var item = await _repo.GetWithEntriesAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (item.IsReconciled)
            throw new InvalidOperationException("Cannot add entries to a reconciled statement.");

        foreach (var entry in entries)
        {
            entry.BankReconciliationId = id;
            await _repo.AddEntryAsync(entry);
        }

        var updated = await _repo.GetWithEntriesAsync(id);
        return MapToDto(updated ?? item);
    }

    public async Task<BankReconciliationDto?> ReconcileAsync(Guid id, CompleteReconciliationDto dto, string reconciledByUserId)
    {
        var item = await _repo.GetWithEntriesAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (item.IsReconciled)
            throw new InvalidOperationException("Already reconciled.");

        // Calculate system balance
        var systemBalance = await CalculateSystemBalanceAsync(item.BankAccountNumber, item.StatementStartDate, item.StatementEndDate);

        item.SystemOpeningBalance = systemBalance.OpeningBalance;
        item.SystemClosingBalance = systemBalance.ClosingBalance;
        item.Difference = item.StatementClosingBalance - systemBalance.ClosingBalance;
        item.IsReconciled = true;
        item.ReconciledAt = DateTime.UtcNow;
        item.ReconciledByUserId = reconciledByUserId;
        item.Notes = dto.Notes;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);

        // Create adjustment journal entry if needed
        if (Math.Abs(item.Difference) > 0.01m)
        {
            await CreateAdjustmentJournalEntryAsync(item, reconciledByUserId);
        }

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

    private async Task<(decimal OpeningBalance, decimal ClosingBalance)> CalculateSystemBalanceAsync(string accountNumber, DateTime startDate, DateTime endDate)
    {
        // This would query the JournalEntry table for bank transactions
        // Simplified implementation
        var entries = await _journalRepo.GetByDateRangeAsync(startDate, endDate);

        decimal openingBalance = 0;
        decimal closingBalance = 0;

        foreach (var entry in entries)
        {
            // Filter by bank account logic here
            closingBalance += entry.LineItems.Sum(li => li.Debit - li.Credit);
        }

        return (openingBalance, closingBalance);
    }

    private async Task CreateAdjustmentJournalEntryAsync(BankReconciliation reconciliation, string userId)
    {
        var adjustmentEntry = new JournalEntry
        {
            EntryNumber = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            EntryDate = DateTime.UtcNow,
            Description = $"Bank reconciliation adjustment for {reconciliation.BankAccountName} as of {reconciliation.StatementDate:d}",
            IsPosted = true,
            PostedDate = DateTime.UtcNow,
            CreatedByUserId = userId,
            ApprovedByUserId = userId,
            ApprovedAt = DateTime.UtcNow
        };

        await _journalRepo.AddAsync(adjustmentEntry);
    }

    private static BankReconciliationDto MapToDto(BankReconciliation br) => new()
    {
        Id = br.Id,
        BankAccountName = br.BankAccountName,
        BankAccountNumber = br.BankAccountNumber,
        StatementDate = br.StatementDate,
        StatementStartDate = br.StatementStartDate,
        StatementEndDate = br.StatementEndDate,
        StatementOpeningBalance = br.StatementOpeningBalance,
        StatementClosingBalance = br.StatementClosingBalance,
        SystemOpeningBalance = br.SystemOpeningBalance,
        SystemClosingBalance = br.SystemClosingBalance,
        Difference = br.Difference,
        IsReconciled = br.IsReconciled,
        ReconciledAt = br.ReconciledAt,
        ReconciledByUserId = br.ReconciledByUserId,
        Notes = br.Notes,
        Entries = br.Entries?.Select(e => new ReconciliationEntryDto
        {
            Id = e.Id,
            TransactionType = e.TransactionType,
            TransactionId = e.TransactionId,
            TransactionDate = e.TransactionDate,
            Amount = e.Amount,
            IsMatched = e.IsMatched,
            Notes = e.Notes
        }).ToList() ?? new(),
        CreatedAt = br.CreatedAt
    };
}