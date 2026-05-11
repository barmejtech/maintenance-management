using Maintenance_management.domain.Enums;

namespace Maintenance_management.domain.Entities;

public enum MeterType
{
    Electricity = 0,
    Water = 1,
    Gas = 2,
    Solar = 3
}

public class MeterReading : BaseEntity
{
    public Guid UnitId { get; set; }
    public Unit? Unit { get; set; }

    public Guid? EquipmentId { get; set; }  // Optional: if meter is on equipment
    public Equipment? Equipment { get; set; }

    public MeterType Type { get; set; }
    public double ReadingValue { get; set; }
    public double? PreviousReadingValue { get; set; }
    public double? Consumption { get; set; }  // Calculated: ReadingValue - PreviousReadingValue
    public DateTime ReadingDate { get; set; } = DateTime.UtcNow;
    public string? PhotoUrl { get; set; }
    public string ReadByUserId { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // For billing
    public decimal? UnitPrice { get; set; }
    public decimal? CalculatedAmount { get; set; }
    public Guid? GeneratedInvoiceId { get; set; }
    public Invoice? GeneratedInvoice { get; set; }
}