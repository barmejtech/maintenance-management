using Maintenance_management.application.DTOs.MaintenanceSchedule;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceSchedulesController : ControllerBase
{
    private readonly IMaintenanceScheduleService _service;

    public MaintenanceSchedulesController(IMaintenanceScheduleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("equipment/{equipmentId:guid}")]
    public async Task<IActionResult> GetByEquipment(Guid equipmentId)
        => Ok(await _service.GetByEquipmentIdAsync(equipmentId));

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
        => Ok(await _service.GetActiveSchedulesAsync());

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
        => Ok(await _service.GetOverdueSchedulesAsync());

    [HttpGet("due-soon")]
    public async Task<IActionResult> GetDueSoon([FromQuery] int withinDays = 7)
        => Ok(await _service.GetDueSoonAsync(withinDays));

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceScheduleDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceScheduleDto dto)
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
