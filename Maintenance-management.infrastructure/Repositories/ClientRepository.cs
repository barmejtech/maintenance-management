using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Client>> GetAllAsync()
        => await _dbSet
            .Include(c => c.MaintenanceRequests.Where(r => !r.IsDeleted))
            .Where(c => !c.IsDeleted)
            .ToListAsync();

    public async Task<Client?> GetWithRequestsAsync(Guid id)
        => await _dbSet
            .Include(c => c.MaintenanceRequests.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

    public async Task<IEnumerable<Client>> SearchAsync(string query)
        => await _dbSet
            .Where(c => !c.IsDeleted &&
                (c.Name.Contains(query) ||
                 (c.CompanyName != null && c.CompanyName.Contains(query)) ||
                 c.Email.Contains(query)))
            .ToListAsync();
}
