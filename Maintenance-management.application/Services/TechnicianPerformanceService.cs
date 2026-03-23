using Maintenance_management.application.DTOs.TechnicianPerformance;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class TechnicianPerformanceService : ITechnicianPerformanceService
{
    private readonly ITechnicianPerformanceScoreRepository _repo;
    private readonly ITechnicianRepository _technicianRepo;
    private readonly ITaskOrderRepository _taskOrderRepo;

    public TechnicianPerformanceService(
        ITechnicianPerformanceScoreRepository repo,
        ITechnicianRepository technicianRepo,
        ITaskOrderRepository taskOrderRepo)
    {
        _repo = repo;
        _technicianRepo = technicianRepo;
        _taskOrderRepo = taskOrderRepo;
    }

    public async Task<IEnumerable<TechnicianPerformanceScoreDto>> GetAllAsync()
    {
        var all = await _repo.GetAllAsync();
        return all.Where(s => !s.IsDeleted).Select(MapToDto);
    }

    public async Task<TechnicianPerformanceScoreDto?> GetByTechnicianIdAsync(Guid technicianId)
    {
        var score = await _repo.GetByTechnicianIdAsync(technicianId);
        return score is null || score.IsDeleted ? null : MapToDto(score);
    }

    public async Task<IEnumerable<TechnicianPerformanceScoreDto>> GetTopPerformersAsync(int count = 10)
    {
        var top = await _repo.GetTopPerformersAsync(count);
        return top.Select(MapToDto);
    }

    public async Task<TechnicianPerformanceScoreDto> RecalculateAsync(Guid technicianId)
    {
        var technician = await _technicianRepo.GetByIdAsync(technicianId);
        if (technician is null || technician.IsDeleted)
            throw new KeyNotFoundException($"Technician {technicianId} not found.");

        var allTasks = await _taskOrderRepo.GetByTechnicianIdAsync(technicianId);
        var tasks = allTasks.Where(t => !t.IsDeleted).ToList();

        var completedTasks = tasks.Where(t => t.Status == domain.Enums.TaskStatus.Completed).ToList();
        var totalCompleted = completedTasks.Count;
        var totalAll = tasks.Count(t => t.Status != domain.Enums.TaskStatus.Cancelled);

        double successRate = totalAll > 0 ? (double)totalCompleted / totalAll * 100 : 0;

        var tasksWithTime = completedTasks
            .Where(t => t.CompletedDate.HasValue && t.ScheduledDate.HasValue)
            .ToList();

        double avgTime = tasksWithTime.Count > 0
            ? tasksWithTime.Average(t => (t.CompletedDate!.Value - t.ScheduledDate!.Value).TotalMinutes)
            : 0;

        var delayedTasks = completedTasks
            .Where(t => t.DueDate.HasValue && t.CompletedDate.HasValue && t.CompletedDate.Value > t.DueDate.Value)
            .Count();

        double onTimeRate = totalCompleted > 0
            ? (double)(totalCompleted - delayedTasks) / totalCompleted * 100
            : 0;

        var existing = await _repo.GetByTechnicianIdAsync(technicianId);
        if (existing is not null)
        {
            existing.AverageInterventionTimeMinutes = Math.Max(0, avgTime);
            existing.SuccessRate = successRate;
            existing.OnTimeRate = onTimeRate;
            existing.TotalTasksCompleted = totalCompleted;
            existing.TotalTasksDelayed = delayedTasks;
            existing.LastCalculatedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(existing);
            return MapToDto(existing);
        }
        else
        {
            var newScore = new TechnicianPerformanceScore
            {
                TechnicianId = technicianId,
                AverageInterventionTimeMinutes = Math.Max(0, avgTime),
                SuccessRate = successRate,
                CustomerSatisfactionScore = 0,
                OnTimeRate = onTimeRate,
                TotalTasksCompleted = totalCompleted,
                TotalTasksDelayed = delayedTasks,
                LastCalculatedAt = DateTime.UtcNow
            };
            var created = await _repo.AddAsync(newScore);
            return MapToDto(created);
        }
    }

    public async Task<bool> UpdateCustomerSatisfactionAsync(Guid technicianId, double score)
    {
        if (score < 0 || score > 5)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 5.");

        var existing = await _repo.GetByTechnicianIdAsync(technicianId);
        if (existing is null) return false;

        existing.CustomerSatisfactionScore = score;
        existing.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(existing);
        return true;
    }

    public async Task<SmartDispatchResultDto?> GetBestTechnicianForTaskAsync(Guid taskOrderId)
    {
        var task = await _taskOrderRepo.GetByIdAsync(taskOrderId);
        if (task is null || task.IsDeleted) return null;

        var availableTechs = await _technicianRepo.GetAvailableTechniciansAsync();
        var techList = availableTechs.ToList();
        if (!techList.Any()) return null;

        var scored = new List<(Technician tech, double score, string reason)>();

        foreach (var tech in techList)
        {
            double distanceScore = 100;
            double perfScore = 50;
            var reasons = new List<string>();

            if (tech.Latitude.HasValue && tech.Longitude.HasValue)
            {
                distanceScore = 80;
                reasons.Add("GPS location available");
            }

            var perf = await _repo.GetByTechnicianIdAsync(tech.Id);
            if (perf is not null)
            {
                perfScore = (perf.SuccessRate * 0.4) +
                            (perf.OnTimeRate * 0.3) +
                            (perf.CustomerSatisfactionScore / 5.0 * 100 * 0.3);
                reasons.Add($"Performance score: {perfScore:F1}");
            }

            double skillScore = 0;
            if (!string.IsNullOrEmpty(tech.Specialization) &&
                task.Description.Contains(tech.Specialization, StringComparison.OrdinalIgnoreCase))
            {
                skillScore = 20;
                reasons.Add($"Skill match: {tech.Specialization}");
            }

            double urgencyBonus = task.Priority switch
            {
                domain.Enums.TaskPriority.Critical => 15,
                domain.Enums.TaskPriority.High => 10,
                _ => 0
            };

            double overall = (distanceScore * 0.3) + (perfScore * 0.5) + skillScore + urgencyBonus;
            scored.Add((tech, overall, string.Join("; ", reasons)));
        }

        var best = scored.OrderByDescending(s => s.score).First();
        var bestPerf = await _repo.GetByTechnicianIdAsync(best.tech.Id);

        return new SmartDispatchResultDto
        {
            TechnicianId = best.tech.Id,
            TechnicianName = $"{best.tech.FirstName} {best.tech.LastName}",
            Specialization = best.tech.Specialization,
            Latitude = best.tech.Latitude,
            Longitude = best.tech.Longitude,
            DistanceScore = 0,
            PerformanceScore = bestPerf is not null ? bestPerf.SuccessRate : 0,
            OverallScore = best.score,
            AssignmentReason = best.reason
        };
    }

    private static TechnicianPerformanceScoreDto MapToDto(TechnicianPerformanceScore s) => new()
    {
        Id = s.Id,
        TechnicianId = s.TechnicianId,
        TechnicianName = s.Technician is not null
            ? $"{s.Technician.FirstName} {s.Technician.LastName}"
            : string.Empty,
        AverageInterventionTimeMinutes = s.AverageInterventionTimeMinutes,
        SuccessRate = s.SuccessRate,
        CustomerSatisfactionScore = s.CustomerSatisfactionScore,
        OnTimeRate = s.OnTimeRate,
        TotalTasksCompleted = s.TotalTasksCompleted,
        TotalTasksDelayed = s.TotalTasksDelayed,
        LastCalculatedAt = s.LastCalculatedAt,
        CreatedAt = s.CreatedAt
    };
}
