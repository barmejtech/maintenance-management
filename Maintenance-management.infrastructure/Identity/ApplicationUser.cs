using Maintenance_management.domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Maintenance_management.infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Client-specific fields (only populated for users with the "Client" role)
    public ClientType? ClientType { get; set; }
    public string? CompanyName { get; set; }
    public Guid? ClientRecordId { get; set; }
}
