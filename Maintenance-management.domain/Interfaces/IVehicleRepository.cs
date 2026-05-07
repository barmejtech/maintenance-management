using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);
    Task<IEnumerable<Vehicle>> GetDueForServiceAsync();
}
