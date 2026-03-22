using Maintenance_management.application.DTOs.Availability;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AvailabilitiesController : ControllerBase
{
    private readonly IAvailabilityService _service;

    public AvailabilitiesController(IAvailabilityService service) => _service = service;

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

    [HttpGet("range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        => Ok(await _service.GetByDateRangeAsync(from, to));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAvailabilityDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAvailabilityDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
