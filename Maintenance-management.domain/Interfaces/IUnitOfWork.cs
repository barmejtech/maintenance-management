using Maintenance_management.domain.Entities;

namespace Maintenance_management.domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
    ITechnicianRepository Technicians { get; }
    ITaskOrderRepository TaskOrders { get; }
    IEquipmentRepository Equipment { get; }
    IGroupRepository Groups { get; }
    IHVACSystemRepository HVACSystems { get; }
    IInvoiceRepository Invoices { get; }
    IReportRepository Reports { get; }
    IAvailabilityRepository Availabilities { get; }
    IDocumentRepository Documents { get; }
    Task<int> SaveChangesAsync();
}
