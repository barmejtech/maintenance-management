using Maintenance_management.application.DTOs.Account;

namespace Maintenance_management.application.Interfaces;

public interface IAccountService
{
    Task<AccountProfileDto> GetProfileAsync(string userId);
    Task<AccountProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
}
