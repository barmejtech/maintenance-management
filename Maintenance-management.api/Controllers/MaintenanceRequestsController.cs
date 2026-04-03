using Maintenance_management.application.DTOs.MaintenanceRequest;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Enums;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance_management.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceRequestsController : ControllerBase
{
    private readonly IMaintenanceRequestService _service;
    private readonly ITechnicianService _technicianService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public MaintenanceRequestsController(
        IMaintenanceRequestService service,
        ITechnicianService technicianService,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _service = service;
        _technicianService = technicianService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll(
        [FromQuery] MaintenanceRequestStatus? status = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var all = await _service.GetAllAsync();

        if (status.HasValue)
            all = all.Where(r => r.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            all = all.Where(r =>
                r.Title.ToLower().Contains(q) ||
                r.ClientName.ToLower().Contains(q) ||
                r.Id.ToString().StartsWith(q, StringComparison.OrdinalIgnoreCase));
        }

        if (from.HasValue)
            all = all.Where(r => r.RequestDate >= from.Value);

        if (to.HasValue)
            all = all.Where(r => r.RequestDate <= to.Value.AddDays(1));

        return Ok(all);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/audit")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAuditLog(Guid id)
        => Ok(await _service.GetAuditLogAsync(id));

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
        => Ok(await _service.GetByClientIdAsync(clientId));

    /// <summary>Returns the maintenance requests belonging to the currently logged-in client.</summary>
    [Authorize(Roles = "Client")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ClientRecordId is null)
            return Ok(Array.Empty<object>());

        var result = await _service.GetByClientIdAsync(user.ClientRecordId.Value);
        return Ok(result);
    }

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

    /// <summary>Clients submit their own maintenance requests (ClientRecordId is resolved from their JWT).</summary>
    [Authorize(Roles = "Client")]
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitRequest([FromBody] SubmitMaintenanceRequestDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ClientRecordId is null)
            return BadRequest(new { message = "Your account is not linked to a client record. Please contact support." });

        var createDto = new CreateMaintenanceRequestDto
        {
            Title = dto.Title,
            Description = dto.Description,
            EquipmentDescription = dto.EquipmentDescription,
            RequestDate = dto.RequestDate,
            ClientId = user.ClientRecordId.Value,
            Notes = dto.Notes
        };

        var result = await _service.CreateAsync(createDto);

        // Notify admins and managers in real-time
        var clientName = $"{user.FirstName} {user.LastName}";
        await _notificationService.SendToRoleAsync("Admin",
            "New Maintenance Request",
            $"Client \"{clientName}\" submitted a new request: \"{result.Title}\"",
            "info", result.Id.ToString(), "MaintenanceRequest");
        await _notificationService.SendToRoleAsync("Manager",
            "New Maintenance Request",
            $"Client \"{clientName}\" submitted a new request: \"{result.Title}\"",
            "info", result.Id.ToString(), "MaintenanceRequest");

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Approves a pending maintenance request.</summary>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveMaintenanceRequestDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        var userName = user is not null ? $"{user.FirstName} {user.LastName}" : userId;

        var result = await _service.ApproveAsync(id, userId, userName, dto.ReviewNotes);
        if (result is null) return NotFound();

        // Notify the client
        var clientRequest = await _service.GetByIdAsync(id);
        if (clientRequest != null)
        {
            // Find the client user
            var clientUsers = await _userManager.GetUsersInRoleAsync("Client");
            var clientUser = clientUsers.FirstOrDefault(u => u.ClientRecordId == clientRequest.ClientId);
            if (clientUser != null)
            {
                await _notificationService.SendToUserAsync(clientUser.Id,
                    "Request Approved",
                    $"Your maintenance request \"{result.Title}\" has been approved.",
                    "success", id.ToString(), "MaintenanceRequest");
            }
        }

        return Ok(result);
    }

    /// <summary>Rejects a pending maintenance request.</summary>
    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectMaintenanceRequestDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        var userName = user is not null ? $"{user.FirstName} {user.LastName}" : userId;

        var result = await _service.RejectAsync(id, userId, userName, dto.ReviewNotes);
        if (result is null) return NotFound();

        // Notify the client
        var clientRequest = await _service.GetByIdAsync(id);
        if (clientRequest != null)
        {
            var clientUsers = await _userManager.GetUsersInRoleAsync("Client");
            var clientUser = clientUsers.FirstOrDefault(u => u.ClientRecordId == clientRequest.ClientId);
            if (clientUser != null)
            {
                await _notificationService.SendToUserAsync(clientUser.Id,
                    "Request Rejected",
                    $"Your maintenance request \"{result.Title}\" has been rejected. Reason: {dto.ReviewNotes ?? "No reason provided."}",
                    "error", id.ToString(), "MaintenanceRequest");
            }
        }

        return Ok(result);
    }

    /// <summary>Assigns one or more available technicians to an approved request.</summary>
    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignTechnicians(Guid id, [FromBody] AssignTechniciansDto dto)
    {
        if (dto.TechnicianIds == null || dto.TechnicianIds.Count == 0)
            return BadRequest(new { message = "At least one technician must be selected." });

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        var userName = user is not null ? $"{user.FirstName} {user.LastName}" : userId;

        var result = await _service.AssignTechniciansAsync(id, dto.TechnicianIds, userId, userName);
        if (result is null) return NotFound();

        // Notify each assigned technician in real-time
        foreach (var techId in dto.TechnicianIds)
        {
            var tech = await _technicianService.GetByIdAsync(techId);
            if (tech is not null)
            {
                var techUser = await _userManager.FindByIdAsync(tech.UserId);
                if (techUser != null)
                {
                    await _notificationService.SendToUserAsync(techUser.Id,
                        "New Assignment",
                        $"You have been assigned to maintenance request: \"{result.Title}\"",
                        "info", id.ToString(), "MaintenanceRequest");
                }
            }
        }

        return Ok(result);
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
