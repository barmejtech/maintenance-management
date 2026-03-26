using Maintenance_management.application.DTOs.DataEntry;

namespace Maintenance_management.application.Interfaces;

public interface IDataEntryService
{
    Task<IEnumerable<DataEntryDto>> GetAllAsync();
    Task<DataEntryDto?> GetByIdAsync(Guid id);
    Task<DataEntryDto> CreateAsync(CreateDataEntryDto dto, string createdByUserId);
    Task<DataEntryDto?> UpdateAsync(Guid id, UpdateDataEntryDto dto);
    Task<bool> DeleteAsync(Guid id);
}
