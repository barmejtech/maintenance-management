using Maintenance_management.application.Interfaces;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Maintenance_management.infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<(bool Success, string UserId, string[] Errors)> CreateUserAsync(
        string email, string firstName, string lastName, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, role);
        return (true, user.Id, Array.Empty<string>());
    }

    public async Task<(bool Success, string[] Errors)> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, new[] { "User not found." });

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Email;
    }
}
