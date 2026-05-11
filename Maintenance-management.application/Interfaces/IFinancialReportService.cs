using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IFinancialReportService
{
    Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate);
    Task<ProfitLossDto> GetProfitLossAsync(DateTime startDate, DateTime endDate);
    Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate);
    Task<CashFlowDto> GetCashFlowAsync(DateTime startDate, DateTime endDate);
    Task<AgingReportDto> GetAccountsReceivableAgingAsync(DateTime asOfDate);
}