using Maintenance_management.application.DTOs.TaskOrder;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskOrdersController : ControllerBase
{
    private readonly ITaskOrderService _service;
    private readonly ITechnicianPerformanceService _performanceService;
    private readonly INotificationService _notificationService;
    private readonly ITechnicianService _technicianService;

    public TaskOrdersController(
        ITaskOrderService service,
        ITechnicianPerformanceService performanceService,
        INotificationService notificationService,
        ITechnicianService technicianService)
    {
        _service = service;
        _performanceService = performanceService;
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

    [HttpGet("technician/{technicianId:guid}")]
    public async Task<IActionResult> GetByTechnician(Guid technicianId)
        => Ok(await _service.GetByTechnicianIdAsync(technicianId));

    [HttpGet("group/{groupId:guid}")]
    public async Task<IActionResult> GetByGroup(Guid groupId)
        => Ok(await _service.GetByGroupIdAsync(groupId));

    [HttpGet("equipment/{equipmentId:guid}")]
    public async Task<IActionResult> GetByEquipment(Guid equipmentId)
        => Ok(await _service.GetByEquipmentIdAsync(equipmentId));

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(domain.Enums.TaskStatus status)
        => Ok(await _service.GetByStatusAsync(status));

    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(await _service.GetScheduledBetweenAsync(from, to));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskOrderDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var result = await _service.CreateAsync(dto, userId);

        // Notify admins and managers about the new work order
        await _notificationService.SendToRoleAsync("Admin",
            "New Work Order Created",
            $"A new work order \"{result.Title}\" has been created.",
            "info", result.Id.ToString(), "TaskOrder");
        await _notificationService.SendToRoleAsync("Manager",
            "New Work Order Created",
            $"A new work order \"{result.Title}\" has been created.",
            "info", result.Id.ToString(), "TaskOrder");

        // Notify the assigned technician if one is provided
        if (dto.TechnicianId.HasValue)
        {
            var technician = await _technicianService.GetByIdAsync(dto.TechnicianId.Value);
            if (technician is not null)
            {
                await _notificationService.SendToUserAsync(technician.UserId,
                    "Work Order Assigned to You",
                    $"You have been assigned to work order \"{result.Title}\".",
                    "success", result.Id.ToString(), "TaskOrder");
            }
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskOrderDto dto)
    {
        // Fetch the current state before updating so we can detect technician and status changes.
        // This extra query is intentional: change-detection logic belongs at the controller boundary
        // to keep the service layer free of notification concerns.
        var oldTask = await _service.GetByIdAsync(id);
        var result = await _service.UpdateAsync(id, dto);
        if (result is null) return NotFound();

        // Notify when a technician is newly assigned or changed
        if (dto.TechnicianId.HasValue &&
            dto.TechnicianId != oldTask?.TechnicianId)
        {
            var technician = await _technicianService.GetByIdAsync(dto.TechnicianId.Value);
            if (technician is not null)
            {
                await _notificationService.SendToUserAsync(technician.UserId,
                    "Work Order Assigned to You",
                    $"You have been assigned to work order \"{result.Title}\".",
                    "success", result.Id.ToString(), "TaskOrder");
            }

            await _notificationService.SendToRoleAsync("Admin",
                "Technician Assigned to Work Order",
                $"Technician assigned to work order \"{result.Title}\".",
                "info", result.Id.ToString(), "TaskOrder");
            await _notificationService.SendToRoleAsync("Manager",
                "Technician Assigned to Work Order",
                $"Technician assigned to work order \"{result.Title}\".",
                "info", result.Id.ToString(), "TaskOrder");
        }

        // Notify when status changes
        if (oldTask is not null && dto.Status != oldTask.Status)
        {
            switch (dto.Status)
            {
                case domain.Enums.TaskStatus.Completed:
                    await _notificationService.SendToRoleAsync("Admin",
                        "Work Order Completed",
                        $"Work order \"{result.Title}\" has been marked as completed.",
                        "success", result.Id.ToString(), "TaskOrder");
                    await _notificationService.SendToRoleAsync("Manager",
                        "Work Order Completed",
                        $"Work order \"{result.Title}\" has been marked as completed.",
                        "success", result.Id.ToString(), "TaskOrder");
                    if (!string.IsNullOrEmpty(result.CreatedByUserId))
                        await _notificationService.SendToUserAsync(result.CreatedByUserId,
                            "Work Order Completed",
                            $"Work order \"{result.Title}\" has been completed.",
                            "success", result.Id.ToString(), "TaskOrder");
                    break;

                case domain.Enums.TaskStatus.Cancelled:
                    await _notificationService.SendToRoleAsync("Admin",
                        "Work Order Cancelled",
                        $"Work order \"{result.Title}\" has been cancelled.",
                        "warning", result.Id.ToString(), "TaskOrder");
                    await _notificationService.SendToRoleAsync("Manager",
                        "Work Order Cancelled",
                        $"Work order \"{result.Title}\" has been cancelled.",
                        "warning", result.Id.ToString(), "TaskOrder");
                    if (result.TechnicianId.HasValue)
                    {
                        var technician = await _technicianService.GetByIdAsync(result.TechnicianId.Value);
                        if (technician is not null)
                            await _notificationService.SendToUserAsync(technician.UserId,
                                "Work Order Cancelled",
                                $"Work order \"{result.Title}\" assigned to you has been cancelled.",
                                "warning", result.Id.ToString(), "TaskOrder");
                    }
                    break;

                case domain.Enums.TaskStatus.InProgress:
                    await _notificationService.SendToRoleAsync("Admin",
                        "Work Order In Progress",
                        $"Work order \"{result.Title}\" is now in progress.",
                        "info", result.Id.ToString(), "TaskOrder");
                    await _notificationService.SendToRoleAsync("Manager",
                        "Work Order In Progress",
                        $"Work order \"{result.Title}\" is now in progress.",
                        "info", result.Id.ToString(), "TaskOrder");
                    break;
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

    [HttpGet("{id:guid}/smart-dispatch")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SmartDispatch(Guid id)
    {
        var result = await _performanceService.GetBestTechnicianForTaskAsync(id);
        return result is null ? NotFound(new { message = "No available technicians found." }) : Ok(result);
    }

    [HttpPatch("{id:guid}/intervention-proof")]
    public async Task<IActionResult> SubmitInterventionProof(Guid id, [FromBody] SubmitInterventionProofDto dto)
    {
        var result = await _service.SubmitInterventionProofAsync(id, dto);
        if (result is null) return NotFound();

        // Notify admins and managers that an intervention proof was submitted
        await _notificationService.SendToRoleAsync("Admin",
            "Intervention Proof Submitted",
            $"Proof of intervention submitted for work order \"{result.Title}\".",
            "success", result.Id.ToString(), "TaskOrder");
        await _notificationService.SendToRoleAsync("Manager",
            "Intervention Proof Submitted",
            $"Proof of intervention submitted for work order \"{result.Title}\".",
            "success", result.Id.ToString(), "TaskOrder");

        return Ok(result);
    }
}
