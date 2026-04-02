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
public class DashboardController : ControllerBase
{
    private readonly IMaintenanceRequestService _requestService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IMaintenanceRequestService requestService,
        UserManager<ApplicationUser> userManager)
    {
        _requestService = requestService;
        _userManager = userManager;
    }

    /// <summary>Returns dashboard statistics scoped to the current user's role.</summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var isClient = roles.Contains("Client");

        if (isClient)
        {
            // Client-specific dashboard: only their own requests
            if (user.ClientRecordId is null)
                return Ok(new ClientDashboardDto());

            var clientRequests = (await _requestService.GetByClientIdAsync(user.ClientRecordId.Value)).ToList();
            var recent = clientRequests
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    r.Status,
                    r.RequestDate,
                    r.CreatedAt
                });

            return Ok(new
            {
                totalRequests = clientRequests.Count,
                pendingRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.Pending),
                underReviewRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.UnderReview),
                approvedRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.Approved),
                inProgressRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.InProgress),
                completedRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.Completed),
                rejectedRequests = clientRequests.Count(r => r.Status == MaintenanceRequestStatus.Rejected),
                recentRequests = recent,
                clientName = $"{user.FirstName} {user.LastName}",
                companyName = user.CompanyName
            });
        }
        else
        {
            // Admin/Manager/Support dashboard: all requests
            var allRequests = (await _requestService.GetAllAsync()).ToList();
            var recent = allRequests
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    r.Status,
                    r.ClientName,
                    r.RequestDate,
                    r.CreatedAt
                });

            return Ok(new
            {
                totalRequests = allRequests.Count,
                pendingRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.Pending),
                underReviewRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.UnderReview),
                approvedRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.Approved),
                inProgressRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.InProgress),
                completedRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.Completed),
                rejectedRequests = allRequests.Count(r => r.Status == MaintenanceRequestStatus.Rejected),
                recentRequests = recent
            });
        }
    }
}

public class ClientDashboardDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int RejectedRequests { get; set; }
}
