using Maintenance_management.application.DTOs.Vehicle;

namespace Maintenance_management.application.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllAsync();
    Task<VehicleDto?> GetByIdAsync(Guid id);
    Task<VehicleDto> CreateAsync(CreateVehicleDto dto);
    Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<VehicleDto>> GetDueForServiceAsync();
}
