using Maintenance_management.application.DTOs.TechnicianPerformance;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechnicianPerformanceController : ControllerBase
{
    private readonly ITechnicianPerformanceService _service;

    public TechnicianPerformanceController(ITechnicianPerformanceService service)
        => _service = service;

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("technician/{technicianId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetByTechnicianId(Guid technicianId)
    {
        var result = await _service.GetByTechnicianIdAsync(technicianId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("top")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetTopPerformers([FromQuery] int count = 10)
        => Ok(await _service.GetTopPerformersAsync(count));

    [HttpPost("recalculate/{technicianId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Recalculate(Guid technicianId)
    {
        try
        {
            var result = await _service.RecalculateAsync(technicianId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("satisfaction/{technicianId:guid}")]
    public async Task<IActionResult> UpdateSatisfaction(Guid technicianId, [FromBody] UpdateCustomerSatisfactionDto dto)
    {
        try
        {
            var result = await _service.UpdateCustomerSatisfactionAsync(technicianId, dto.Score);
            return result ? NoContent() : NotFound();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
