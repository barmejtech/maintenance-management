using Maintenance_management.application.DTOs.Payment;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    [HttpGet("request/{requestId:guid}")]
    public async Task<IActionResult> GetByRequest(Guid requestId)
    {
        var result = await _service.GetByRequestIdAsync(requestId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentDto dto)
    {
        try
        {
            var result = await _service.InitiatePaymentAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{paymentId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid paymentId, [FromBody] ConfirmPaymentDto dto)
    {
        var result = await _service.ConfirmPaymentAsync(paymentId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{paymentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid paymentId)
    {
        var result = await _service.CancelPaymentAsync(paymentId);
        return result is null ? NotFound() : Ok(result);
    }
}
