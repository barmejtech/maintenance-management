using Maintenance_management.application.DTOs.Auth;

namespace Maintenance_management.application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(string userId, string email, IList<string> roles);
    string GenerateRefreshToken();
    string? GetUserIdFromExpiredToken(string token);
}
