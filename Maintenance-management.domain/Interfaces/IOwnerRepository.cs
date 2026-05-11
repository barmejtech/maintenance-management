using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IOwnerRepository : IRepository<Owner>
{
    Task<Owner?> GetWithHistoryAsync(Guid id);
}
