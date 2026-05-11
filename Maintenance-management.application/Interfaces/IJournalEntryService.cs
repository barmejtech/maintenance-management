using Maintenance_management.application.DTOs.NewEntities;

namespace Maintenance_management.application.Interfaces;

public interface IJournalEntryService
{
    Task<IEnumerable<JournalEntryDto>> GetAllAsync();
    Task<JournalEntryDto?> GetByIdAsync(Guid id);
    Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto, string createdByUserId);
    Task<JournalEntryDto?> UpdateAsync(Guid id, CreateJournalEntryDto dto);
    Task<JournalEntryDto?> PostAsync(Guid id, string approvedByUserId);
    Task<bool> DeleteAsync(Guid id);
}