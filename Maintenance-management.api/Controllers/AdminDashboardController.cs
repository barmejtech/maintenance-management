using Maintenance_management.application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public AdminDashboardController(IAdminDashboardService service)
    {
        _service = service;
    }

    /// <summary>Returns the admin dashboard data: request rows, daily stats and weekly average.</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.GetDashboardAsync();
        return Ok(data);
    }
}
