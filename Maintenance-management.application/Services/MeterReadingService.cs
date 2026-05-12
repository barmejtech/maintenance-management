using Maintenance_management.application.DTOs.NewEntities;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class MeterReadingService : IMeterReadingService
{
    private readonly IMeterReadingRepository _repo;
    private readonly IUnitRepository _unitRepo;

    public MeterReadingService(IMeterReadingRepository repo, IUnitRepository unitRepo)
    {
        _repo = repo;
        _unitRepo = unitRepo;
    }

    public async Task<IEnumerable<MeterReadingDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<MeterReadingDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null || item.IsDeleted ? null : MapToDto(item);
    }

    public async Task<IEnumerable<MeterReadingDto>> GetByUnitIdAsync(Guid unitId)
    {
        var items = await _repo.GetByUnitIdAsync(unitId);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<MeterReadingDto>> GetByTypeAsync(MeterType type)
    {
        var items = await _repo.GetByTypeAsync(type);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<MeterReadingDto> CreateAsync(CreateMeterReadingDto dto, string readByUserId)
    {
        var previousReading = await _repo.GetLatestByUnitAndTypeAsync(dto.UnitId, dto.Type);

        var entity = new MeterReading
        {
            UnitId = dto.UnitId,
            EquipmentId = dto.EquipmentId,
            Type = dto.Type,
            ReadingValue = dto.ReadingValue,
            PreviousReadingValue = previousReading?.ReadingValue,
            Consumption = previousReading != null ? dto.ReadingValue - previousReading.ReadingValue : null,
            ReadingDate = dto.ReadingDate,
            PhotoUrl = dto.PhotoUrl,
            ReadByUserId = readByUserId,
            Notes = dto.Notes,
            UnitPrice = dto.UnitPrice,
            CalculatedAmount = dto.UnitPrice.HasValue
                ? (decimal)(dto.ReadingValue - (previousReading?.ReadingValue ?? 0)) * dto.UnitPrice.Value
                : null
        };

        var created = await _repo.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<MeterReadingDto?> UpdateAsync(Guid id, UpdateMeterReadingDto dto)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item is null || item.IsDeleted) return null;

        item.ReadingValue = dto.ReadingValue;
        item.PhotoUrl = dto.PhotoUrl ?? item.PhotoUrl;
        item.Notes = dto.Notes ?? item.Notes;
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

   
    public async Task<BulkMeterReadingResultDto> BulkCreateAsync(BulkMeterReadingDto dto, string readByUserId)
    {
        var results = new List<MeterReadingDto>();
        var errors = new List<string>();

        foreach (var reading in dto.Readings)
        {
            try
            {
                var createDto = new CreateMeterReadingDto
                {
                    UnitId = reading.UnitId,
                    Type = dto.Type,
                    ReadingValue = reading.ReadingValue,
                    ReadingDate = dto.ReadingDate,
                    UnitPrice = dto.UnitPrice
                };
                var created = await CreateAsync(createDto, readByUserId);
                results.Add(created);
            }
            catch (Exception ex)
            {
                errors.Add($"Unit {reading.UnitId}: {ex.Message}");
            }
        }

        return new BulkMeterReadingResultDto
        {
            SuccessCount = results.Count,
            Errors = errors,
            Readings = results
        };
    }

    private static MeterReadingDto MapToDto(MeterReading m) => new()
    {
        Id = m.Id,
        UnitId = m.UnitId,
        UnitNumber = m.Unit?.UnitNumber ?? string.Empty,
        EquipmentId = m.EquipmentId,
        EquipmentName = m.Equipment?.Name,
        Type = m.Type,
        ReadingValue = m.ReadingValue,
        PreviousReadingValue = m.PreviousReadingValue,
        Consumption = m.Consumption,
        ReadingDate = m.ReadingDate,
        PhotoUrl = m.PhotoUrl,
        ReadByUserId = m.ReadByUserId,
        Notes = m.Notes,
        UnitPrice = m.UnitPrice,
        CalculatedAmount = m.CalculatedAmount,
        GeneratedInvoiceId = m.GeneratedInvoiceId,
        CreatedAt = m.CreatedAt
    };

    public async Task<IEnumerable<MeterReadingDto>> GetByTypeAsync(domain.Enums.MeterType type)
    {
        // Map Enums.MeterType to Entities.MeterType by name
        if (!Enum.TryParse<MeterType>(type.ToString(), out var entityType))
            return Enumerable.Empty<MeterReadingDto>();

        var items = await _repo.GetByTypeAsync(entityType);
        return items.Where(x => !x.IsDeleted).Select(MapToDto);
    }

    public async Task<MeterReadingChartDataDto> GetChartDataAsync(Guid unitId, domain.Enums.MeterType type, int months = 12)
    {
        if (!Enum.TryParse<MeterType>(type.ToString(), out var entityType))
            return new MeterReadingChartDataDto { Label = type.ToString() };

        var readings = await _repo.GetByUnitAndTypeWithDateRangeAsync(unitId, entityType, months);
        var readingList = readings.Where(r => !r.IsDeleted).OrderBy(r => r.ReadingDate).ToList();

        return new MeterReadingChartDataDto
        {
            Label = type.ToString(),
            ReadingValue = readingList.LastOrDefault()?.ReadingValue ?? 0,
            Consumption = readingList.Sum(r => r.Consumption ?? 0),
            Amount = readingList.Sum(r => r.CalculatedAmount ?? 0)
        };
    }
}