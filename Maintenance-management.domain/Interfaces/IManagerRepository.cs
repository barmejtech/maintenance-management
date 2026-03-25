using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IManagerRepository : IRepository<Manager>
{
    Task<Manager?> GetByUserIdAsync(string userId);
}
