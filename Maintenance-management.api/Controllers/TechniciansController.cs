using Maintenance_management.application.DTOs.Technician;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechniciansController : ControllerBase
{
    private readonly ITechnicianService _service;
    private readonly INotificationService _notificationService;

    public TechniciansController(ITechnicianService service, INotificationService notificationService)
    {
        _service = service;
        _notificationService = notificationService;
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

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateTechnicianDto dto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var result = await _service.CreateAsync(dto, userId);

            // Notify admins and managers about the new technician
            await _notificationService.SendToRoleAsync("Admin",
                "New Technician Registered",
                $"A new technician \"{result.FullName}\" ({result.Specialization}) has been added.",
                "success", result.Id.ToString(), "Technician");
            await _notificationService.SendToRoleAsync("Manager",
                "New Technician Registered",
                $"A new technician \"{result.FullName}\" ({result.Specialization}) has been added.",
                "success", result.Id.ToString(), "Technician");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTechnicianDto dto)
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

    [HttpPatch("{id:guid}/location")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationDto dto)
    {
        var result = await _service.UpdateLocationAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentTechnician()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var result = await _service.GetByUserIdAsync(userId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("available")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAvailable()
        => Ok(await _service.GetAvailableAsync());
}
