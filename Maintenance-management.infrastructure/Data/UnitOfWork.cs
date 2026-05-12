using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Repositories;

namespace Maintenance_management.infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    public UnitOfWork(
        ApplicationDbContext context,
        ITechnicianRepository technicians,
        ITaskOrderRepository taskOrders,
        IEquipmentRepository equipment,
        IVehicleRepository vehicles,
        IGroupRepository groups,
        IHVACSystemRepository hvacSystems,
        IInvoiceRepository invoices,
        IReportRepository reports,
        IAvailabilityRepository availabilities,
        IDocumentRepository documents,
        IUnitTypeRepository unitTypes,
        IUnitRepository units,
        IOwnerRepository owners,
        IUnitOwnershipRepository unitOwnerships,
        ITenantRepository tenants,
        IMaintenanceRequestRepository maintenanceRequests,
        IAccountRepository accounts,
        IJournalEntryRepository journalEntries,
        IVendorRepository vendors,
        IExpenseRepository expenses,
        IPaymentVoucherRepository paymentVouchers,
        IBankReconciliationRepository bankReconciliations,
        IMeterReadingRepository meterReadings,
        IRenovationRepository renovations)
    {
        _context = context;
        Technicians = technicians;
        TaskOrders = taskOrders;
        Equipment = equipment;
        Vehicles = vehicles;
        Groups = groups;
        HVACSystems = hvacSystems;
        Invoices = invoices;
        Reports = reports;
        Availabilities = availabilities;
        Documents = documents;
        UnitTypes = unitTypes;
        Units = units;
        Owners = owners;
        UnitOwnerships = unitOwnerships;
        Tenants = tenants;
        MaintenanceRequests = maintenanceRequests;
        Accounts = accounts;
        JournalEntries = journalEntries;
        Vendors = vendors;
        Expenses = expenses;
        PaymentVouchers = paymentVouchers;
        BankReconciliations = bankReconciliations;
        MeterReadings = meterReadings;
        Renovations = renovations;
    }

    public ITechnicianRepository Technicians { get; }
    public ITaskOrderRepository TaskOrders { get; }
    public IEquipmentRepository Equipment { get; }
    public IVehicleRepository Vehicles { get; }
    public IGroupRepository Groups { get; }
    public IHVACSystemRepository HVACSystems { get; }
    public IInvoiceRepository Invoices { get; }
    public IReportRepository Reports { get; }
    public IAvailabilityRepository Availabilities { get; }
    public IDocumentRepository Documents { get; }

    // Property domain
    public IUnitTypeRepository UnitTypes { get; }
    public IUnitRepository Units { get; }
    public IOwnerRepository Owners { get; }
    public IUnitOwnershipRepository UnitOwnerships { get; }
    public ITenantRepository Tenants { get; }
    public IMaintenanceRequestRepository MaintenanceRequests { get; }

    // Accounting domain
    public IAccountRepository Accounts { get; }
    public IJournalEntryRepository JournalEntries { get; }
    public IVendorRepository Vendors { get; }
    public IExpenseRepository Expenses { get; }
    public IPaymentVoucherRepository PaymentVouchers { get; }
    public IBankReconciliationRepository BankReconciliations { get; }

    // Operational
    public IMeterReadingRepository MeterReadings { get; }
    public IRenovationRepository Renovations { get; }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
