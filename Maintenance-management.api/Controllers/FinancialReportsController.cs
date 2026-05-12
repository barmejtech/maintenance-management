using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/financial-reports")]
[Authorize]
public class FinancialReportsController : ControllerBase
{
    private readonly IFinancialReportService _service;

    public FinancialReportsController(IFinancialReportService service) => _service = service;

    [HttpGet("trial-balance")]
    public async Task<IActionResult> GetTrialBalance([FromQuery] DateTime? asOfDate)
        => Ok(await _service.GetTrialBalanceAsync(asOfDate ?? DateTime.UtcNow));

    [HttpGet("profit-loss")]
    public async Task<IActionResult> GetProfitLoss([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(await _service.GetProfitLossAsync(startDate, endDate));

    [HttpGet("balance-sheet")]
    public async Task<IActionResult> GetBalanceSheet([FromQuery] DateTime? asOfDate)
        => Ok(await _service.GetBalanceSheetAsync(asOfDate ?? DateTime.UtcNow));

    [HttpGet("cash-flow")]
    public async Task<IActionResult> GetCashFlow([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(await _service.GetCashFlowAsync(startDate, endDate));

    [HttpGet("aging-report")]
    public async Task<IActionResult> GetAgingReport([FromQuery] DateTime? asOfDate)
        => Ok(await _service.GetAccountsReceivableAgingAsync(asOfDate ?? DateTime.UtcNow));
}
