using Maintenance_management.application.DTOs.Client;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _repo;

    public ClientService(IClientRepository repo) => _repo = repo;

    public async Task<IEnumerable<ClientDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(c => !c.IsDeleted).Select(MapToDto);
    }

    public async Task<ClientDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetWithRequestsAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var entity = new Client
        {
            Name = dto.Name,
            CompanyName = dto.CompanyName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            Notes = dto.Notes
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<ClientDto?> UpdateAsync(Guid id, UpdateClientDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.Name = dto.Name;
        item.CompanyName = dto.CompanyName;
        item.Email = dto.Email;
        item.Phone = dto.Phone;
        item.Address = dto.Address;
        item.Notes = dto.Notes;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return false;

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static ClientDto MapToDto(Client c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        CompanyName = c.CompanyName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        Notes = c.Notes,
        MaintenanceRequestCount = c.MaintenanceRequests?.Count(r => !r.IsDeleted) ?? 0,
        CreatedAt = c.CreatedAt
    };
}
