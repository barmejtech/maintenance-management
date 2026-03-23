using Maintenance_management.application.DTOs.EquipmentDigitalTwin;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentDigitalTwinController : ControllerBase
{
    private readonly IEquipmentDigitalTwinService _service;

    public EquipmentDigitalTwinController(IEquipmentDigitalTwinService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("equipment/{equipmentId:guid}")]
    public async Task<IActionResult> GetByEquipment(Guid equipmentId)
    {
        var result = await _service.GetByEquipmentIdAsync(equipmentId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Upsert([FromBody] UpsertDigitalTwinDto dto)
    {
        try
        {
            var result = await _service.UpsertAsync(dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("sync/{equipmentId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SyncFromEquipment(Guid equipmentId)
    {
        var result = await _service.SyncFromEquipmentAsync(equipmentId);
        return result is null ? NotFound() : Ok(result);
    }
}
