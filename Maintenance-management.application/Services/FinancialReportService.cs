using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly IAccountRepository _accountRepo;
    private readonly IJournalEntryRepository _journalRepo;
    private readonly IInvoiceRepository _invoiceRepo;  // ADD THIS

    public FinancialReportService(
        IAccountRepository accountRepo,
        IJournalEntryRepository journalRepo,
        IInvoiceRepository invoiceRepo)  // ADD THIS PARAMETER
    {
        _accountRepo = accountRepo;
        _journalRepo = journalRepo;
        _invoiceRepo = invoiceRepo;  // ADD THIS
    }

    public async Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate)
    {
        var accounts = await _accountRepo.GetAllAsync();
        var entries = await _journalRepo.GetPostedEntriesUpToDateAsync(asOfDate);

        var result = new TrialBalanceDto();

        foreach (var account in accounts.Where(a => !a.IsDeleted))
        {
            var debit = entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Debit));

            var credit = entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Credit));

            result.Accounts.Add(new TrialBalanceAccountDto
            {
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                Debit = debit,
                Credit = credit
            });
        }

        result.TotalDebit = result.Accounts.Sum(x => x.Debit);
        result.TotalCredit = result.Accounts.Sum(x => x.Credit);

        return result;
    }

    public async Task<ProfitLossDto> GetProfitLossAsync(DateTime startDate, DateTime endDate)
    {
        var accounts = await _accountRepo.GetByTypeAsync(domain.Enums.AccountType.Revenue);
        var expenseAccounts = await _accountRepo.GetByTypeAsync(domain.Enums.AccountType.Expense);

        var entries = await _journalRepo.GetPostedEntriesByDateRangeAsync(startDate, endDate);

        var revenue = new List<AccountBalanceDto>();
        var expenses = new List<AccountBalanceDto>();

        decimal totalRevenue = 0;
        decimal totalExpenses = 0;

        foreach (var account in accounts)
        {
            var credit = entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Credit));

            totalRevenue += credit;
            revenue.Add(new AccountBalanceDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                NetBalance = credit
            });
        }

        foreach (var account in expenseAccounts)
        {
            var debit = entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Debit));

            totalExpenses += debit;
            expenses.Add(new AccountBalanceDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                NetBalance = debit
            });
        }

        return new ProfitLossDto
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            NetProfit = totalRevenue - totalExpenses,
            RevenueAccounts = revenue,
            ExpenseAccounts = expenses,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate)
    {
        var assetAccounts = await _accountRepo.GetByTypeAsync(domain.Enums.AccountType.Asset);
        var liabilityAccounts = await _accountRepo.GetByTypeAsync(domain.Enums.AccountType.Liability);
        var equityAccounts = await _accountRepo.GetByTypeAsync(domain.Enums.AccountType.Equity);

        var entries = await _journalRepo.GetPostedEntriesUpToDateAsync(asOfDate);

        var assets = new List<AccountBalanceDto>();
        var liabilities = new List<AccountBalanceDto>();
        var equities = new List<AccountBalanceDto>();

        decimal totalAssets = 0;
        decimal totalLiabilities = 0;
        decimal totalEquity = 0;

        foreach (var account in assetAccounts)
        {
            var balance = account.OpeningBalance ?? 0;
            balance += entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Debit - li.Credit));

            totalAssets += balance;
            assets.Add(new AccountBalanceDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                NetBalance = balance
            });
        }

        foreach (var account in liabilityAccounts)
        {
            var balance = account.OpeningBalance ?? 0;
            balance += entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Credit - li.Debit));

            totalLiabilities += balance;
            liabilities.Add(new AccountBalanceDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                NetBalance = balance
            });
        }

        foreach (var account in equityAccounts)
        {
            var balance = account.OpeningBalance ?? 0;
            balance += entries.Sum(e => e.LineItems
                .Where(li => li.AccountId == account.Id)
                .Sum(li => li.Credit - li.Debit));

            totalEquity += balance;
            equities.Add(new AccountBalanceDto
            {
                AccountId = account.Id,
                AccountCode = account.AccountCode,
                AccountName = account.Name,
                NetBalance = balance
            });
        }

        return new BalanceSheetDto
        {
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            TotalEquity = totalEquity,
            Assets = assets,
            Liabilities = liabilities,
            Equity = equities,
            AsOfDate = asOfDate
        };
    }

    public async Task<CashFlowDto> GetCashFlowAsync(DateTime startDate, DateTime endDate)
    {
        var entries = await _journalRepo.GetPostedEntriesByDateRangeAsync(startDate, endDate);

        var operating = new List<CashFlowItemDto>();
        var investing = new List<CashFlowItemDto>();
        var financing = new List<CashFlowItemDto>();

        decimal operatingTotal = 0;
        decimal investingTotal = 0;
        decimal financingTotal = 0;

        foreach (var entry in entries)
        {
            foreach (var line in entry.LineItems)
            {
                var amount = line.Debit - line.Credit;

                // FIX: Use .Value or compare with nullable
                if (line.Account?.Type == AccountType.Asset &&
                    line.Account?.Name?.Contains("Cash") == true)
                {
                    operatingTotal += amount;
                    operating.Add(new CashFlowItemDto
                    {
                        Description = entry.Description ?? string.Empty,
                        Amount = amount,
                        Date = entry.EntryDate
                    });
                }
            }
        }

        return new CashFlowDto
        {
            OperatingCashFlow = operatingTotal,
            InvestingCashFlow = investingTotal,
            FinancingCashFlow = financingTotal,
            NetCashFlow = operatingTotal + investingTotal + financingTotal,
            OperatingItems = operating,
            InvestingItems = investing,
            FinancingItems = financing
        };
    }

    public async Task<AgingReportDto> GetAccountsReceivableAgingAsync(DateTime asOfDate)
    {
        var invoices = await _invoiceRepo.GetUnpaidInvoicesAsync(asOfDate);

        var report = new AgingReportDto();

        foreach (var invoice in invoices)
        {
            var daysOverdue = (asOfDate - (invoice.DueDate ?? invoice.IssueDate)).Days;
            var amount = invoice.TotalAmount - (invoice.PaidAmount ?? 0);

            var item = new AgingItemDto
            {
                Id = invoice.Id,
                Name = invoice.ClientName,
                DocumentNumber = invoice.InvoiceNumber,
                DueDate = invoice.DueDate ?? invoice.IssueDate,
                Amount = amount,
                DaysOverdue = Math.Max(0, daysOverdue)
            };

            if (daysOverdue <= 0)
            {
                report.Current += amount;
                item.AgingBucket = "Current";
                report.CurrentItems.Add(item);
            }
            else if (daysOverdue <= 30)
            {
                report.Days1To30 += amount;
                item.AgingBucket = "1-30 Days";
                report.Days1To30Items.Add(item);
            }
            else if (daysOverdue <= 60)
            {
                report.Days31To60 += amount;
                item.AgingBucket = "31-60 Days";
                report.Days31To60Items.Add(item);
            }
            else if (daysOverdue <= 90)
            {
                report.Days61To90 += amount;
                item.AgingBucket = "61-90 Days";
                report.Days61To90Items.Add(item);
            }
            else
            {
                report.Days90Plus += amount;
                item.AgingBucket = "90+ Days";
                report.Days90PlusItems.Add(item);
            }

            report.Total += amount;
            report.Items.Add(item);
        }

        return report;
    }
}