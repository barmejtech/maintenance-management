using Maintenance_management.application.DTOs.Technician;
using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/gps-tracking")]
[Authorize]
public class GpsTrackingController : ControllerBase
{
    private readonly IGpsTrackingService _gpsTrackingService;

    public GpsTrackingController(IGpsTrackingService gpsTrackingService)
    {
        _gpsTrackingService = gpsTrackingService;
    }

    [HttpPost("{technicianId:guid}/update-location")]
    public async Task<IActionResult> UpdateLocation(Guid technicianId, [FromBody] UpdateTechnicianLocationDto dto)
    {
        await _gpsTrackingService.UpdateTechnicianLocationAsync(technicianId, dto.Latitude, dto.Longitude);
        return Ok(new { message = "Location updated successfully." });
    }

    [HttpGet("{technicianId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid technicianId)
    {
        var history = await _gpsTrackingService.GetLocationHistoryAsync(technicianId);
        return Ok(history);
    }

    [HttpGet("{technicianId:guid}/latest")]
    public async Task<IActionResult> GetLatest(Guid technicianId)
    {
        var latest = await _gpsTrackingService.GetLatestLocationAsync(technicianId);
        return latest is null ? NotFound() : Ok(latest);
    }

    [HttpGet("{technicianId:guid}/distance")]
    public async Task<IActionResult> CalculateDistance(
        Guid technicianId,
        [FromQuery] double serviceLatitude,
        [FromQuery] double serviceLongitude)
    {
        var result = await _gpsTrackingService.CalculateDistanceAsync(technicianId, serviceLatitude, serviceLongitude);
        return result is null ? NotFound() : Ok(result);
    }
}
