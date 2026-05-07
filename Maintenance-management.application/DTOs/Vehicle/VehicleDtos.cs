using Maintenance_management.domain.Enums;

namespace Maintenance_management.application.DTOs.Vehicle;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int? Mileage { get; set; }
    public string? EngineType { get; set; }
    public TransmissionType? TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerPhone { get; set; }
    public string? OwnerEmail { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public int? LastServiceMileage { get; set; }
    public int? NextServiceMileage { get; set; }
    public VehicleStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? VehiclePhotoUrl { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleDto
{
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int? Mileage { get; set; }
    public string? EngineType { get; set; }
    public TransmissionType? TransmissionType { get; set; }
    public FuelType FuelType { get; set; } = FuelType.Gasoline;
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerPhone { get; set; }
    public string? OwnerEmail { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public int? NextServiceMileage { get; set; }
    public string? Notes { get; set; }
    public string? VehiclePhotoUrl { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
}

public class UpdateVehicleDto
{
    public string VIN { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int? Mileage { get; set; }
    public string? EngineType { get; set; }
    public TransmissionType? TransmissionType { get; set; }
    public FuelType FuelType { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerPhone { get; set; }
    public string? OwnerEmail { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? LastServiceDate { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public int? LastServiceMileage { get; set; }
    public int? NextServiceMileage { get; set; }
    public VehicleStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? VehiclePhotoUrl { get; set; }
    public string? Photo2Url { get; set; }
    public string? Photo3Url { get; set; }
    public string? Photo4Url { get; set; }
    public string? QrCode { get; set; }
}
