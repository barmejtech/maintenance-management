using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IDataEntryRepository : IRepository<DataEntry>
{
    Task<DataEntry?> GetByUserIdAsync(string userId);
}
