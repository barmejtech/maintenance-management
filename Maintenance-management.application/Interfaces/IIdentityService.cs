namespace Maintenance_management.application.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, string[] Errors)> CreateUserAsync(
        string email, string firstName, string lastName, string password, string role);
    Task<(bool Success, string[] Errors)> DeleteUserAsync(string userId);
    Task<string?> GetUserEmailAsync(string userId);
}
