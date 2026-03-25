using Maintenance_management.application.DTOs.User;

namespace Maintenance_management.application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
}
