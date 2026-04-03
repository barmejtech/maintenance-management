using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TravelEstimationController : ControllerBase
{
    private readonly ITravelEstimationService _service;

    public TravelEstimationController(ITravelEstimationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Estimates the driving distance and travel time from a technician's
    /// current GPS location to a client's registered address.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Technician")]
    public async Task<IActionResult> Estimate([FromQuery] Guid technicianId, [FromQuery] Guid clientId)
    {
        if (technicianId == Guid.Empty || clientId == Guid.Empty)
            return BadRequest(new { message = "technicianId and clientId are required." });

        var result = await _service.EstimateAsync(technicianId, clientId);
        if (result is null)
            return NotFound(new { message = "Technician or client not found." });

        if (!result.TechnicianHasLocation)
            return Ok(new
            {
                success = false,
                message = "Technician does not have a registered GPS location.",
                data = result
            });

        if (string.IsNullOrWhiteSpace(result.ClientAddress))
            return Ok(new
            {
                success = false,
                message = "Client does not have a registered address.",
                data = result
            });

        if (string.IsNullOrWhiteSpace(result.FormattedDistance))
            return Ok(new
            {
                success = false,
                message = "Could not calculate route. Address may be invalid or routing service unavailable.",
                data = result
            });

        return Ok(new { success = true, data = result });
    }
}
