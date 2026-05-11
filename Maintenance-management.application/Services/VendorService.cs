using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repo;

    public VendorService(IVendorRepository repo) => _repo = repo;

    public async Task<IEnumerable<VendorDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<VendorDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<VendorDto> CreateAsync(CreateVendorDto dto)
    {
        if (await _repo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Vendor with this email already exists.");

        var entity = new Vendor
        {
            Name = dto.Name.Trim(),
            CompanyName = dto.CompanyName?.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone,
            Address = dto.Address,
            TaxNumber = dto.TaxNumber,
            BankName = dto.BankName,
            BankAccountNumber = dto.BankAccountNumber,
            Notes = dto.Notes,
            IsActive = true
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<VendorDto?> UpdateAsync(Guid id, UpdateVendorDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        if (await _repo.EmailExistsAsync(dto.Email, id))
            throw new InvalidOperationException("Vendor with this email already exists.");

        item.Name = dto.Name.Trim();
        item.CompanyName = dto.CompanyName?.Trim();
        item.Email = dto.Email.Trim();
        item.Phone = dto.Phone;
        item.Address = dto.Address;
        item.TaxNumber = dto.TaxNumber;
        item.BankName = dto.BankName;
        item.BankAccountNumber = dto.BankAccountNumber;
        item.Notes = dto.Notes;
        item.IsActive = dto.IsActive;
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

    public async Task<IEnumerable<VendorDto>> GetActiveAsync()
    {
        var items = await _repo.GetActiveAsync();
        return items.Select(MapToDto);
    }

    private static VendorDto MapToDto(Vendor v) => new()
    {
        Id = v.Id,
        Name = v.Name,
        CompanyName = v.CompanyName,
        Email = v.Email,
        Phone = v.Phone,
        Address = v.Address,
        TaxNumber = v.TaxNumber,
        BankName = v.BankName,
        BankAccountNumber = v.BankAccountNumber,
        Notes = v.Notes,
        IsActive = v.IsActive,
        CreatedAt = v.CreatedAt
    };
}