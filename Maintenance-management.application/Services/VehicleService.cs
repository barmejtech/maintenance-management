using Maintenance_management.application.DTOs.Vehicle;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repo;

    public VehicleService(IVehicleRepository repo) => _repo = repo;

    public async Task<IEnumerable<VehicleDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(v => !v.IsDeleted).Select(MapToDto);
    }

    public async Task<VehicleDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto)
    {
        var entity = new Vehicle
        {
            VIN = dto.VIN,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            LicensePlate = dto.LicensePlate,
            Color = dto.Color,
            Mileage = dto.Mileage,
            EngineType = dto.EngineType,
            TransmissionType = dto.TransmissionType,
            FuelType = dto.FuelType,
            OwnerName = dto.OwnerName,
            OwnerPhone = dto.OwnerPhone,
            OwnerEmail = dto.OwnerEmail,
            PurchaseDate = dto.PurchaseDate,
            NextServiceDate = dto.NextServiceDate,
            NextServiceMileage = dto.NextServiceMileage,
            Notes = dto.Notes,
            VehiclePhotoUrl = dto.VehiclePhotoUrl,
            Photo2Url = dto.Photo2Url,
            Photo3Url = dto.Photo3Url,
            Photo4Url = dto.Photo4Url,
            QrCode = dto.QrCode ?? GenerateQrCode(dto.Make, dto.Model, dto.Year, dto.VIN)
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.VIN = dto.VIN;
        item.Make = dto.Make;
        item.Model = dto.Model;
        item.Year = dto.Year;
        item.LicensePlate = dto.LicensePlate;
        item.Color = dto.Color;
        item.Mileage = dto.Mileage;
        item.EngineType = dto.EngineType;
        item.TransmissionType = dto.TransmissionType;
        item.FuelType = dto.FuelType;
        item.OwnerName = dto.OwnerName;
        item.OwnerPhone = dto.OwnerPhone;
        item.OwnerEmail = dto.OwnerEmail;
        item.PurchaseDate = dto.PurchaseDate;
        item.LastServiceDate = dto.LastServiceDate;
        item.NextServiceDate = dto.NextServiceDate;
        item.LastServiceMileage = dto.LastServiceMileage;
        item.NextServiceMileage = dto.NextServiceMileage;
        item.Status = dto.Status;
        item.Notes = dto.Notes;
        item.VehiclePhotoUrl = dto.VehiclePhotoUrl;
        item.Photo2Url = dto.Photo2Url;
        item.Photo3Url = dto.Photo3Url;
        item.Photo4Url = dto.Photo4Url;
        item.QrCode = dto.QrCode ?? item.QrCode ?? GenerateQrCode(dto.Make, dto.Model, dto.Year, dto.VIN);
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

    public async Task<IEnumerable<VehicleDto>> GetDueForServiceAsync()
    {
        var items = await _repo.GetDueForServiceAsync();
        return items.Select(MapToDto);
    }

    private static VehicleDto MapToDto(Vehicle v) => new()
    {
        Id = v.Id,
        VIN = v.VIN,
        Make = v.Make,
        Model = v.Model,
        Year = v.Year,
        LicensePlate = v.LicensePlate,
        Color = v.Color,
        Mileage = v.Mileage,
        EngineType = v.EngineType,
        TransmissionType = v.TransmissionType,
        FuelType = v.FuelType,
        OwnerName = v.OwnerName,
        OwnerPhone = v.OwnerPhone,
        OwnerEmail = v.OwnerEmail,
        PurchaseDate = v.PurchaseDate,
        LastServiceDate = v.LastServiceDate,
        NextServiceDate = v.NextServiceDate,
        LastServiceMileage = v.LastServiceMileage,
        NextServiceMileage = v.NextServiceMileage,
        Status = v.Status,
        Notes = v.Notes,
        VehiclePhotoUrl = v.VehiclePhotoUrl,
        Photo2Url = v.Photo2Url,
        Photo3Url = v.Photo3Url,
        Photo4Url = v.Photo4Url,
        QrCode = v.QrCode,
        CreatedAt = v.CreatedAt
    };

    private static string GenerateQrCode(string make, string model, int year, string vin)
    {
        var parts = new[] { $"{year}", make, model, vin }
            .Where(p => !string.IsNullOrEmpty(p));
        return string.Join(' ', parts);
    }
}
