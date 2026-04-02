using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly IMaintenanceRequestService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public MaintenanceRequestsController(IMaintenanceRequestService service, UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
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

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
        => Ok(await _service.GetByClientIdAsync(clientId));

    /// <summary>Returns the maintenance requests belonging to the currently logged-in client.</summary>
    [Authorize(Roles = "Client")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ClientRecordId is null)
            return Ok(Array.Empty<object>());

        var result = await _service.GetByClientIdAsync(user.ClientRecordId.Value);
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(MaintenanceRequestStatus status)
        => Ok(await _service.GetByStatusAsync(status));

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequestDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Clients submit their own maintenance requests (ClientRecordId is resolved from their JWT).</summary>
    [Authorize(Roles = "Client")]
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitRequest([FromBody] SubmitMaintenanceRequestDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ClientRecordId is null)
            return BadRequest(new { message = "Your account is not linked to a client record. Please contact support." });

        var createDto = new CreateMaintenanceRequestDto
        {
            Title = dto.Title,
            Description = dto.Description,
            EquipmentDescription = dto.EquipmentDescription,
            RequestDate = dto.RequestDate,
            ClientId = user.ClientRecordId.Value,
            Notes = dto.Notes
        };

        var result = await _service.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceRequestDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
