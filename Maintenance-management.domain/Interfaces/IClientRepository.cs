using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetWithRequestsAsync(Guid id);
    Task<IEnumerable<Client>> SearchAsync(string query);
}
