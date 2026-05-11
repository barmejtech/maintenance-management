using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IVendorRepository : IRepository<Vendor>
{
    Task<IEnumerable<Vendor>> GetActiveAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
}