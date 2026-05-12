using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
    ITechnicianRepository Technicians { get; }
    ITaskOrderRepository TaskOrders { get; }
    IEquipmentRepository Equipment { get; }
    IVehicleRepository Vehicles { get; }
    IGroupRepository Groups { get; }
    IHVACSystemRepository HVACSystems { get; }
    IInvoiceRepository Invoices { get; }
    IReportRepository Reports { get; }
    IAvailabilityRepository Availabilities { get; }
    IDocumentRepository Documents { get; }

    // Property domain
    IUnitTypeRepository UnitTypes { get; }
    IUnitRepository Units { get; }
    IOwnerRepository Owners { get; }
    IUnitOwnershipRepository UnitOwnerships { get; }
    ITenantRepository Tenants { get; }
    IMaintenanceRequestRepository MaintenanceRequests { get; }

    // Accounting domain
    IAccountRepository Accounts { get; }
    IJournalEntryRepository JournalEntries { get; }
    IVendorRepository Vendors { get; }
    IExpenseRepository Expenses { get; }
    IPaymentVoucherRepository PaymentVouchers { get; }
    IBankReconciliationRepository BankReconciliations { get; }

    // Operational
    IMeterReadingRepository MeterReadings { get; }
    IRenovationRepository Renovations { get; }

    Task<int> SaveChangesAsync();
}
