using Maintenance_management.domain.Entities;

namespace Maintenance_management.application.DTOs.Unit;

public class UnitOwnershipHistoryDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal OwnershipPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? SaleDate { get; set; }
    public bool IsActive { get; set; }
}

public class UnitDto
{
    public Guid Id { get; set; }
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; }
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
    public string UnitTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<UnitOwnershipHistoryDto> OwnershipHistory { get; set; } = new();
    public List<UnitMaintenanceSummaryDto> RecentMaintenanceRequests { get; set; } = new();
    public TenantSummaryDto? CurrentTenant { get; set; }
}

public class UnitMaintenanceSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TenantSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime LeaseStartDate { get; set; }
    public DateTime LeaseEndDate { get; set; }
}

public class CreateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; } = UnitStatus.Vacant;
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
}

public class UpdateUnitDto
{
    public string UnitNumber { get; set; } = string.Empty;
    public int? Floor { get; set; }
    public decimal? SizeSqm { get; set; }
    public UnitStatus Status { get; set; }
    public decimal? ShareValue { get; set; }
    public Guid UnitTypeId { get; set; }
}

public class UnitMassUpdateDto
{
    public List<Guid> UnitIds { get; set; } = new();
    public UnitStatus? Status { get; set; }
    public int? Floor { get; set; }
}