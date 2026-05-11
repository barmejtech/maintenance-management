using Maintenance_management.application.DTOs.Owner;

namespace Maintenance_management.application.Interfaces;

public interface IOwnerService
{
    Task<IEnumerable<OwnerDto>> GetAllAsync();
    Task<OwnerDto?> GetByIdAsync(Guid id);
    Task<OwnerDto> CreateAsync(CreateOwnerDto dto);
    Task<OwnerDto?> UpdateAsync(Guid id, UpdateOwnerDto dto);
    Task<bool> TransferOwnershipAsync(Guid ownerId, TransferOwnershipDto dto, string performedByUserId, string? performedByName);
    Task<bool> DeleteAsync(Guid id);
}
