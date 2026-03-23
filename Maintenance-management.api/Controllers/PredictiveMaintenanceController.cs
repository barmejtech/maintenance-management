using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PredictiveMaintenanceController : ControllerBase
{
    private readonly IPredictiveMaintenanceService _service;

    public PredictiveMaintenanceController(IPredictiveMaintenanceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllPredictionsAsync());

    [HttpGet("equipment/{equipmentId:guid}")]
    public async Task<IActionResult> GetByEquipment(Guid equipmentId)
    {
        var result = await _service.GetPredictionByEquipmentAsync(equipmentId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("run/{equipmentId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RunPrediction(Guid equipmentId)
    {
        try
        {
            var result = await _service.RunPredictionAsync(equipmentId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("high-risk")]
    public async Task<IActionResult> GetHighRisk([FromQuery] double threshold = 0.7)
        => Ok(await _service.GetHighRiskEquipmentAsync(threshold));
}
