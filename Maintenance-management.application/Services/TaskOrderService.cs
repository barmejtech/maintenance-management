using Maintenance_management.application.DTOs.TaskOrder;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class TaskOrderService : ITaskOrderService
{
    private readonly ITaskOrderRepository _repo;
    private readonly ITechnicianRepository _technicianRepo;
    private readonly INotificationService _notificationService;

    public TaskOrderService(
        ITaskOrderRepository repo,
        ITechnicianRepository technicianRepo,
        INotificationService notificationService)
    {
        _repo = repo;
        _technicianRepo = technicianRepo;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<TaskOrderDto>> GetAllAsync()
    {
        var tasks = await _repo.GetAllAsync();
        return tasks.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<TaskOrderDto?> GetByIdAsync(Guid id)
    {
        var task = await _repo.GetWithDetailsAsync(id);
        return task is null || task.IsDeleted ? null : MapToDto(task);
    }

    public async Task<IEnumerable<TaskOrderDto>> GetByTechnicianIdAsync(Guid technicianId)
    {
        var tasks = await _repo.GetByTechnicianIdAsync(technicianId);
        return tasks.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<TaskOrderDto>> GetByGroupIdAsync(Guid groupId)
    {
        var tasks = await _repo.GetByGroupIdAsync(groupId);
        return tasks.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<TaskOrderDto>> GetByStatusAsync(domain.Enums.TaskStatus status)
    {
        var tasks = await _repo.GetByStatusAsync(status);
        return tasks.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<IEnumerable<TaskOrderDto>> GetScheduledBetweenAsync(DateTime from, DateTime to)
    {
        var tasks = await _repo.GetScheduledBetweenAsync(from, to);
        return tasks.Where(t => !t.IsDeleted).Select(MapToDto);
    }

    public async Task<TaskOrderDto> CreateAsync(CreateTaskOrderDto dto, string createdByUserId)
    {
        var entity = new TaskOrder
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            MaintenanceType = dto.MaintenanceType,
            ScheduledDate = dto.ScheduledDate,
            DueDate = dto.DueDate,
            Notes = dto.Notes,
            TechnicianId = dto.TechnicianId,
            GroupId = dto.GroupId,
            EquipmentId = dto.EquipmentId,
            CreatedByUserId = createdByUserId
        };

        var created = await _repo.AddAsync(entity);

        // Notify the assigned technician
        if (dto.TechnicianId.HasValue)
        {
            var technician = await _technicianRepo.GetByIdAsync(dto.TechnicianId.Value);
            if (technician is not null && !string.IsNullOrEmpty(technician.UserId))
            {
                await _notificationService.SendToUserAsync(
                    technician.UserId,
                    "New Task Assigned",
                    $"You have been assigned to work order: \"{dto.Title}\"",
                    "info",
                    created.Id.ToString(),
                    "TaskOrder");
            }
        }

        return MapToDto(created);
    }

    public async Task<TaskOrderDto?> UpdateAsync(Guid id, UpdateTaskOrderDto dto)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null || task.IsDeleted) return null;

        var previousStatus = task.Status;
        var previousTechId = task.TechnicianId;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.MaintenanceType = dto.MaintenanceType;
        task.ScheduledDate = dto.ScheduledDate;
        task.DueDate = dto.DueDate;
        task.Notes = dto.Notes;
        task.TechnicianId = dto.TechnicianId;
        task.GroupId = dto.GroupId;
        task.EquipmentId = dto.EquipmentId;
        task.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == domain.Enums.TaskStatus.Completed && task.CompletedDate is null)
            task.CompletedDate = DateTime.UtcNow;

        await _repo.UpdateAsync(task);

        // Notify managers and admins when a job is completed
        if (previousStatus != domain.Enums.TaskStatus.Completed &&
            dto.Status == domain.Enums.TaskStatus.Completed)
        {
            await _notificationService.SendToRoleAsync(
                "Manager",
                "Work Order Completed",
                $"Work order \"{task.Title}\" has been marked as completed.",
                "success",
                task.Id.ToString(),
                "TaskOrder");

            await _notificationService.SendToRoleAsync(
                "Admin",
                "Work Order Completed",
                $"Work order \"{task.Title}\" has been marked as completed.",
                "success",
                task.Id.ToString(),
                "TaskOrder");
        }

        // Notify the newly assigned technician if technician changed
        if (dto.TechnicianId.HasValue && dto.TechnicianId != previousTechId)
        {
            var technician = await _technicianRepo.GetByIdAsync(dto.TechnicianId.Value);
            if (technician is not null && !string.IsNullOrEmpty(technician.UserId))
            {
                await _notificationService.SendToUserAsync(
                    technician.UserId,
                    "Task Assignment Updated",
                    $"You have been assigned to work order: \"{task.Title}\"",
                    "info",
                    task.Id.ToString(),
                    "TaskOrder");
            }
        }

        return MapToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null || task.IsDeleted) return false;

        task.IsDeleted = true;
        task.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(task);
        return true;
    }

    public async Task<TaskOrderDto?> SubmitInterventionProofAsync(Guid id, SubmitInterventionProofDto dto)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null || task.IsDeleted) return null;

        if (dto.ArrivalLatitude.HasValue && dto.ArrivalLongitude.HasValue)
        {
            task.ArrivalLatitude = dto.ArrivalLatitude;
            task.ArrivalLongitude = dto.ArrivalLongitude;
            task.ArrivalTime = DateTime.UtcNow;
            task.IsGpsValidated = true;
        }

        if (!string.IsNullOrEmpty(dto.ProofPhotoUrl))
            task.ProofPhotoUrl = dto.ProofPhotoUrl;

        if (!string.IsNullOrEmpty(dto.CustomerSignatureUrl))
            task.CustomerSignatureUrl = dto.CustomerSignatureUrl;

        task.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(task);
        return MapToDto(task);
    }

    private static TaskOrderDto MapToDto(TaskOrder t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        MaintenanceType = t.MaintenanceType,
        ScheduledDate = t.ScheduledDate,
        DueDate = t.DueDate,
        CompletedDate = t.CompletedDate,
        Notes = t.Notes,
        CreatedByUserId = t.CreatedByUserId,
        TechnicianId = t.TechnicianId,
        TechnicianName = t.Technician is not null
            ? $"{t.Technician.FirstName} {t.Technician.LastName}"
            : null,
        GroupId = t.GroupId,
        GroupName = t.Group?.Name,
        EquipmentId = t.EquipmentId,
        EquipmentName = t.Equipment?.Name,
        CreatedAt = t.CreatedAt,
        ArrivalLatitude = t.ArrivalLatitude,
        ArrivalLongitude = t.ArrivalLongitude,
        ArrivalTime = t.ArrivalTime,
        ProofPhotoUrl = t.ProofPhotoUrl,
        CustomerSignatureUrl = t.CustomerSignatureUrl,
        IsGpsValidated = t.IsGpsValidated
    };
}
