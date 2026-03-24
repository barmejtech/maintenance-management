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

    public TaskOrdersController(ITaskOrderService service, ITechnicianPerformanceService performanceService)
    {
        _service = service;
        _performanceService = performanceService;
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
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskOrderDto dto)
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
        return result is null ? NotFound() : Ok(result);
    }
}
