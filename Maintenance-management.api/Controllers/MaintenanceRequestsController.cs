using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly IMaintenanceRequestService _service;

    public MaintenanceRequestsController(IMaintenanceRequestService service)
        => _service = service;

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
