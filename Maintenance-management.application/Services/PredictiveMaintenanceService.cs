using Maintenance_management.application.DTOs.PredictiveMaintenance;
using Maintenance_management.application.Interfaces;
using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.domain.Interfaces;

namespace Maintenance_management.application.Services;

public class PredictiveMaintenanceService : IPredictiveMaintenanceService
{
    private readonly IEquipmentHealthPredictionRepository _predictionRepo;
    private readonly IEquipmentRepository _equipmentRepo;
    private readonly ITaskOrderRepository _taskOrderRepo;

    public PredictiveMaintenanceService(
        IEquipmentHealthPredictionRepository predictionRepo,
        IEquipmentRepository equipmentRepo,
        ITaskOrderRepository taskOrderRepo)
    {
        _predictionRepo = predictionRepo;
        _equipmentRepo = equipmentRepo;
        _taskOrderRepo = taskOrderRepo;
    }

    public async Task<IEnumerable<EquipmentHealthPredictionDto>> GetAllPredictionsAsync()
    {
        var predictions = await _predictionRepo.GetAllAsync();
        return predictions.Where(p => !p.IsDeleted).Select(MapToDto);
    }

    public async Task<EquipmentHealthPredictionDto?> GetPredictionByEquipmentAsync(Guid equipmentId)
    {
        var prediction = await _predictionRepo.GetByEquipmentIdAsync(equipmentId);
        return prediction is null ? null : MapToDto(prediction);
    }

    public async Task<EquipmentHealthPredictionDto> RunPredictionAsync(Guid equipmentId)
    {
        var equipment = await _equipmentRepo.GetByIdAsync(equipmentId)
            ?? throw new KeyNotFoundException($"Equipment {equipmentId} not found.");

        var tasks = (await _taskOrderRepo.GetByEquipmentIdAsync(equipmentId))
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.CreatedAt)
            .ToList();

        var completedTasks = tasks.Where(t => t.Status == domain.Enums.TaskStatus.Completed).ToList();
        var correctiveTasks = completedTasks.Where(t => t.MaintenanceType == MaintenanceType.Corrective || t.MaintenanceType == MaintenanceType.Emergency).ToList();

        int totalInterventions = completedTasks.Count;
        double avgDaysBetweenFailures = ComputeAverageDaysBetween(correctiveTasks.Select(t => t.CompletedDate ?? t.UpdatedAt ?? t.CreatedAt).ToList());
        double avgDaysBetweenMaintenance = ComputeAverageDaysBetween(completedTasks.Select(t => t.CompletedDate ?? t.UpdatedAt ?? t.CreatedAt).ToList());

        double failureProbability = ComputeFailureProbability(equipment, totalInterventions, avgDaysBetweenFailures, avgDaysBetweenMaintenance);
        DateTime predictedFailureDate = ComputePredictedFailureDate(equipment, avgDaysBetweenFailures);
        string recommendation = GenerateRecommendation(failureProbability, equipment, avgDaysBetweenMaintenance);

        var existing = await _predictionRepo.GetByEquipmentIdAsync(equipmentId);
        EquipmentHealthPrediction prediction;

        if (existing is not null)
        {
            existing.PredictedFailureDate = predictedFailureDate;
            existing.FailureProbability = failureProbability;
            existing.Recommendation = recommendation;
            existing.TotalInterventions = totalInterventions;
            existing.AverageDaysBetweenFailures = avgDaysBetweenFailures;
            existing.AverageDaysBetweenMaintenance = avgDaysBetweenMaintenance;
            existing.LastAnalyzedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            await _predictionRepo.UpdateAsync(existing);
            prediction = existing;
        }
        else
        {
            prediction = new EquipmentHealthPrediction
            {
                EquipmentId = equipmentId,
                PredictedFailureDate = predictedFailureDate,
                FailureProbability = failureProbability,
                Recommendation = recommendation,
                TotalInterventions = totalInterventions,
                AverageDaysBetweenFailures = avgDaysBetweenFailures,
                AverageDaysBetweenMaintenance = avgDaysBetweenMaintenance,
                LastAnalyzedAt = DateTime.UtcNow
            };
            await _predictionRepo.AddAsync(prediction);
        }

        prediction.Equipment = equipment;
        return MapToDto(prediction);
    }

    public async Task<IEnumerable<EquipmentHealthPredictionDto>> GetHighRiskEquipmentAsync(double threshold = 0.7)
    {
        var predictions = await _predictionRepo.GetHighRiskAsync(threshold);
        return predictions.Select(MapToDto);
    }

    // ── private helpers ──────────────────────────────────────────────

    private static double ComputeAverageDaysBetween(IList<DateTime> dates)
    {
        if (dates.Count < 2) return 0;
        var sorted = dates.OrderBy(d => d).ToList();
        var gaps = new List<double>();
        for (int i = 1; i < sorted.Count; i++)
            gaps.Add((sorted[i] - sorted[i - 1]).TotalDays);
        return gaps.Average();
    }

    private static double ComputeFailureProbability(
        Equipment equipment,
        int totalInterventions,
        double avgDaysBetweenFailures,
        double avgDaysBetweenMaintenance)
    {
        double score = 0.0;

        // High intervention frequency increases risk
        if (totalInterventions >= 10) score += 0.35;
        else if (totalInterventions >= 5) score += 0.20;
        else if (totalInterventions >= 2) score += 0.10;

        // Short average time between failures increases risk
        if (avgDaysBetweenFailures > 0 && avgDaysBetweenFailures < 30) score += 0.30;
        else if (avgDaysBetweenFailures >= 30 && avgDaysBetweenFailures < 90) score += 0.15;

        // Overdue maintenance increases risk
        if (equipment.NextMaintenanceDate.HasValue && equipment.NextMaintenanceDate.Value < DateTime.UtcNow)
            score += 0.20;

        // Long gap since last maintenance
        if (equipment.LastMaintenanceDate.HasValue)
        {
            var daysSinceLast = (DateTime.UtcNow - equipment.LastMaintenanceDate.Value).TotalDays;
            if (daysSinceLast > 365) score += 0.15;
            else if (daysSinceLast > 180) score += 0.08;
        }
        else
        {
            score += 0.10; // No recorded maintenance
        }

        // Equipment status
        if (equipment.Status == EquipmentStatus.OutOfService) score += 0.30;
        else if (equipment.Status == EquipmentStatus.UnderMaintenance) score += 0.10;

        return Math.Min(score, 1.0);
    }

    private static DateTime ComputePredictedFailureDate(Equipment equipment, double avgDaysBetweenFailures)
    {
        var baseDate = equipment.LastMaintenanceDate ?? equipment.CreatedAt;
        double daysUntilFailure = avgDaysBetweenFailures > 0 ? avgDaysBetweenFailures : 180;

        // Clamp to a reasonable range
        daysUntilFailure = Math.Max(7, Math.Min(daysUntilFailure, 730));
        return baseDate.AddDays(daysUntilFailure);
    }

    private static string GenerateRecommendation(double probability, Equipment equipment, double avgDaysBetweenMaintenance)
    {
        if (probability >= 0.75)
            return "⚠️ Critical risk: Schedule immediate inspection and preventive maintenance.";
        if (probability >= 0.50)
            return "🔶 High risk: Plan maintenance within the next 2 weeks.";
        if (probability >= 0.30)
            return "🔷 Moderate risk: Monitor closely and schedule maintenance within 30 days.";
        if (avgDaysBetweenMaintenance > 0 && avgDaysBetweenMaintenance < 60)
            return "ℹ️ Low risk but frequent interventions detected. Review maintenance procedures.";
        return "✅ Equipment appears healthy. Continue regular scheduled maintenance.";
    }

    private static EquipmentHealthPredictionDto MapToDto(EquipmentHealthPrediction p) => new()
    {
        Id = p.Id,
        EquipmentId = p.EquipmentId,
        EquipmentName = p.Equipment?.Name ?? string.Empty,
        PredictedFailureDate = p.PredictedFailureDate,
        FailureProbability = p.FailureProbability,
        Recommendation = p.Recommendation,
        TotalInterventions = p.TotalInterventions,
        AverageDaysBetweenFailures = p.AverageDaysBetweenFailures,
        AverageDaysBetweenMaintenance = p.AverageDaysBetweenMaintenance,
        LastAnalyzedAt = p.LastAnalyzedAt,
        CreatedAt = p.CreatedAt
    };
}
