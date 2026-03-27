using Maintenance_management.application.DTOs.Client;

namespace Maintenance_management.application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(Guid id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
    Task<ClientDto?> UpdateAsync(Guid id, UpdateClientDto dto);
    Task<bool> DeleteAsync(Guid id);
}
