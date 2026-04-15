using Maintenance_management.application.DTOs.Quotation;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotationsController : ControllerBase
{
    private readonly IQuotationService _service;
    private readonly IEmailService _emailService;

    public QuotationsController(IQuotationService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(QuotationStatus status)
        => Ok(await _service.GetByStatusAsync(status));

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
        => Ok(await _service.GetByClientIdAsync(clientId));

    [HttpGet("request/{requestId:guid}")]
    public async Task<IActionResult> GetByMaintenanceRequest(Guid requestId)
        => Ok(await _service.GetByMaintenanceRequestIdAsync(requestId));

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateQuotationDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuotationDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/send-email")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SendEmail(Guid id)
    {
        var quotation = await _service.GetByIdAsync(id);
        if (quotation is null) return NotFound();

        if (string.IsNullOrWhiteSpace(quotation.ClientEmail))
            return BadRequest(new { message = "Client email address is required to send the quotation." });

        await _emailService.SendQuotationEmailAsync(quotation.ClientEmail, quotation);

        // Mark as Sent if still in Draft status
        if (quotation.Status == QuotationStatus.Draft)
        {
            var updateDto = new UpdateQuotationDto
            {
                ClientName = quotation.ClientName,
                ClientEmail = quotation.ClientEmail,
                ClientAddress = quotation.ClientAddress,
                ClientPhone = quotation.ClientPhone,
                ValidUntil = quotation.ValidUntil,
                EstimatedDurationDays = quotation.EstimatedDurationDays,
                TaxRate = quotation.TaxRate,
                Notes = quotation.Notes,
                TermsAndConditions = quotation.TermsAndConditions,
                MaintenanceRequestId = quotation.MaintenanceRequestId,
                ClientId = quotation.ClientId,
                Status = QuotationStatus.Sent
            };
            await _service.UpdateAsync(id, updateDto);
        }

        return Ok(new { message = "Quotation sent successfully." });
    }
}
