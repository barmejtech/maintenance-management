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
        IGroupRepository groups,
        IHVACSystemRepository hvacSystems,
        IInvoiceRepository invoices,
        IReportRepository reports,
        IAvailabilityRepository availabilities,
        IDocumentRepository documents)
    {
        _context = context;
        Technicians = technicians;
        TaskOrders = taskOrders;
        Equipment = equipment;
        Groups = groups;
        HVACSystems = hvacSystems;
        Invoices = invoices;
        Reports = reports;
        Availabilities = availabilities;
        Documents = documents;
    }

    public ITechnicianRepository Technicians { get; }
    public ITaskOrderRepository TaskOrders { get; }
    public IEquipmentRepository Equipment { get; }
    public IGroupRepository Groups { get; }
    public IHVACSystemRepository HVACSystems { get; }
    public IInvoiceRepository Invoices { get; }
    public IReportRepository Reports { get; }
    public IAvailabilityRepository Availabilities { get; }
    public IDocumentRepository Documents { get; }

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
