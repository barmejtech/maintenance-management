using Maintenance_management.application.DTOs.User;
using Maintenance_management.application.Interfaces;

namespace Maintenance_management.application.Services;

public class UserService : IUserService
{
    private readonly IIdentityService _identityService;

    public UserService(IIdentityService identityService) => _identityService = identityService;

    public async Task<IEnumerable<UserDto>> GetAllAsync()
        => await _identityService.GetAllUsersWithRolesAsync();
}
