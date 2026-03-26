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
    private readonly INotificationService _notificationService;
    private readonly ITechnicianService _technicianService;

    public MaintenanceSchedulesController(
        IMaintenanceScheduleService service,
        INotificationService notificationService,
        ITechnicianService technicianService)
    {
        _service = service;
        _notificationService = notificationService;
        _technicianService = technicianService;
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

        // Notify admins and managers about the new maintenance schedule
        await _notificationService.SendToRoleAsync("Admin",
            "New Maintenance Schedule",
            $"A new maintenance schedule \"{result.Name}\" has been created.",
            "info", result.Id.ToString(), "MaintenanceSchedule");
        await _notificationService.SendToRoleAsync("Manager",
            "New Maintenance Schedule",
            $"A new maintenance schedule \"{result.Name}\" has been created.",
            "info", result.Id.ToString(), "MaintenanceSchedule");

        // Notify the assigned technician
        if (dto.AssignedTechnicianId.HasValue)
        {
            var technician = await _technicianService.GetByIdAsync(dto.AssignedTechnicianId.Value);
            if (technician is not null)
            {
                await _notificationService.SendToUserAsync(technician.UserId,
                    "Maintenance Schedule Assigned to You",
                    $"You have been assigned to maintenance schedule \"{result.Name}\".",
                    "success", result.Id.ToString(), "MaintenanceSchedule");
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaintenanceScheduleDto dto)
    {
        var oldSchedule = await _service.GetByIdAsync(id);
        var result = await _service.UpdateAsync(id, dto);
        if (result is null) return NotFound();

        // Notify technician if assignment changed
        if (dto.AssignedTechnicianId.HasValue &&
            dto.AssignedTechnicianId != oldSchedule?.AssignedTechnicianId)
        {
            var technician = await _technicianService.GetByIdAsync(dto.AssignedTechnicianId.Value);
            if (technician is not null)
            {
                await _notificationService.SendToUserAsync(technician.UserId,
                    "Maintenance Schedule Assigned to You",
                    $"You have been assigned to maintenance schedule \"{result.Name}\".",
                    "success", result.Id.ToString(), "MaintenanceSchedule");
            }
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
