using Maintenance_management.domain.Entities;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<DataEntry> DataEntries => Set<DataEntry>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<TechnicianGroup> TechnicianGroups => Set<TechnicianGroup>();
    public DbSet<TechnicianGroupMember> TechnicianGroupMembers => Set<TechnicianGroupMember>();
    public DbSet<TaskOrder> TaskOrders => Set<TaskOrder>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<HVACSystem> HVACSystems => Set<HVACSystem>();
    public DbSet<MaintenanceReport> MaintenanceReports => Set<MaintenanceReport>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<AppNotification> AppNotifications => Set<AppNotification>();
    public DbSet<EquipmentHealthPrediction> EquipmentHealthPredictions => Set<EquipmentHealthPrediction>();
    public DbSet<EquipmentDigitalTwin> EquipmentDigitalTwins => Set<EquipmentDigitalTwin>();
    public DbSet<TechnicianPerformanceScore> TechnicianPerformanceScores => Set<TechnicianPerformanceScore>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<SparePartUsage> SparePartUsages => Set<SparePartUsage>();
    public DbSet<MaintenanceSchedule> MaintenanceSchedules => Set<MaintenanceSchedule>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<MaintenanceRequestAssignment> MaintenanceRequestAssignments => Set<MaintenanceRequestAssignment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<PremiumService> PremiumServices => Set<PremiumService>();
    public DbSet<PremiumMaintenanceRequest> PremiumMaintenanceRequests => Set<PremiumMaintenanceRequest>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TechnicianGpsLog> TechnicianGpsLogs => Set<TechnicianGpsLog>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationLineItem> QuotationLineItems => Set<QuotationLineItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Manager>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
            e.Property(m => m.LastName).IsRequired().HasMaxLength(100);
            e.Property(m => m.Email).IsRequired().HasMaxLength(256);
            e.Property(m => m.Phone).HasMaxLength(50);
            e.Property(m => m.Department).HasMaxLength(200);
            e.HasIndex(m => m.UserId).IsUnique();
        });

        builder.Entity<DataEntry>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
            e.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            e.Property(d => d.Email).IsRequired().HasMaxLength(256);
            e.Property(d => d.Phone).HasMaxLength(50);
            e.Property(d => d.Section).HasMaxLength(200);
            e.HasIndex(d => d.UserId).IsUnique();
        });

        builder.Entity<Technician>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
            e.Property(t => t.LastName).IsRequired().HasMaxLength(100);
            e.Property(t => t.Email).IsRequired().HasMaxLength(256);
            e.Property(t => t.Phone).HasMaxLength(50);
            e.Property(t => t.Specialization).HasMaxLength(200);
            e.HasIndex(t => t.UserId).IsUnique();
        });

        builder.Entity<TechnicianGroup>(e =>
        {
            e.HasKey(g => g.Id);
            e.Property(g => g.Name).IsRequired().HasMaxLength(100);
            e.Property(g => g.Description).HasMaxLength(500);
        });

        builder.Entity<TechnicianGroupMember>(e =>
        {
            e.HasKey(m => m.Id);
            e.HasOne(m => m.Technician)
                .WithMany(t => t.GroupMembers)
                .HasForeignKey(m => m.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(m => new { m.TechnicianId, m.GroupId }).IsUnique();
        });

        builder.Entity<TaskOrder>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.HasOne(t => t.Technician)
                .WithMany(tech => tech.Tasks)
                .HasForeignKey(t => t.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Group)
                .WithMany(g => g.Tasks)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(t => t.Equipment)
                .WithMany(eq => eq.Tasks)
                .HasForeignKey(t => t.EquipmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Equipment>(e =>
        {
            e.HasKey(eq => eq.Id);
            e.Property(eq => eq.Name).IsRequired().HasMaxLength(200);
            e.Property(eq => eq.SerialNumber).HasMaxLength(100);
        });

        builder.Entity<HVACSystem>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.Name).IsRequired().HasMaxLength(200);
            e.HasOne(h => h.Equipment)
                .WithOne(eq => eq.HVACSystem)
                .HasForeignKey<HVACSystem>(h => h.EquipmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<MaintenanceReport>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Title).IsRequired().HasMaxLength(200);
            e.HasOne(r => r.TaskOrder)
                .WithMany(t => t.Reports)
                .HasForeignKey(r => r.TaskOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Invoice>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
            e.Property(i => i.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(i => i.TaxRate).HasColumnType("decimal(5,2)");
            e.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
            e.HasOne(i => i.TaskOrder)
                .WithOne(t => t.Invoice)
                .HasForeignKey<Invoice>(i => i.TaskOrderId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(i => i.MaintenanceReport)
                .WithMany(r => r.Invoices)
                .HasForeignKey(i => i.MaintenanceReportId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<InvoiceLineItem>(e =>
        {
            e.HasKey(li => li.Id);
            e.Property(li => li.Quantity).HasColumnType("decimal(10,2)");
            e.Property(li => li.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(li => li.Total).HasColumnType("decimal(18,2)");
            e.HasOne(li => li.Invoice)
                .WithMany(i => i.LineItems)
                .HasForeignKey(li => li.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Availability>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Technician)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(a => a.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Document>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.FileName).IsRequired().HasMaxLength(500);
            e.Property(d => d.ContentType).HasMaxLength(100);
            e.HasOne(d => d.Technician)
                .WithMany(t => t.Documents)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.TaskOrder)
                .WithMany(t => t.Documents)
                .HasForeignKey(d => d.TaskOrderId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.Equipment)
                .WithMany(eq => eq.Documents)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.MaintenanceReport)
                .WithMany(r => r.Documents)
                .HasForeignKey(d => d.MaintenanceReportId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(d => d.HVACSystem)
                .WithMany(h => h.Documents)
                .HasForeignKey(d => d.HVACSystemId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ChatMessage>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.SenderId).IsRequired().HasMaxLength(450);
            e.Property(m => m.SenderName).IsRequired().HasMaxLength(200);
            e.Property(m => m.ReceiverId).HasMaxLength(450);
            e.Property(m => m.Content).HasMaxLength(4000);
            e.Property(m => m.FileUrl).HasMaxLength(1000);
            e.Property(m => m.FileName).HasMaxLength(500);
            e.Property(m => m.ContentType).HasMaxLength(100);
        });

        builder.Entity<AppNotification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.UserId).IsRequired().HasMaxLength(450);
            e.Property(n => n.Title).IsRequired().HasMaxLength(200);
            e.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            e.Property(n => n.RelatedEntityId).HasMaxLength(450);
            e.Property(n => n.RelatedEntityType).HasMaxLength(100);
            e.HasIndex(n => new { n.UserId, n.IsDeleted });
        });

        builder.Entity<EquipmentHealthPrediction>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Recommendation).HasMaxLength(1000);
            e.HasOne(p => p.Equipment)
                .WithMany()
                .HasForeignKey(p => p.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => p.EquipmentId);
        });

        builder.Entity<EquipmentDigitalTwin>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.LastKnownIssue).HasMaxLength(1000);
            e.Property(t => t.SimulationNotes).HasMaxLength(2000);
            e.HasOne(t => t.Equipment)
                .WithMany()
                .HasForeignKey(t => t.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(t => t.EquipmentId).IsUnique();
        });

        builder.Entity<TechnicianPerformanceScore>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.Technician)
                .WithMany()
                .HasForeignKey(s => s.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => s.TechnicianId).IsUnique();
        });

        builder.Entity<SparePart>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.PartNumber).HasMaxLength(100);
            e.Property(p => p.Description).HasMaxLength(1000);
            e.Property(p => p.Unit).HasMaxLength(50);
            e.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.Supplier).HasMaxLength(200);
            e.Property(p => p.StorageLocation).HasMaxLength(200);
            e.Property(p => p.Notes).HasMaxLength(1000);
        });

        builder.Entity<SparePartUsage>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.UsedByUserId).IsRequired().HasMaxLength(450);
            e.Property(u => u.Notes).HasMaxLength(500);
            e.HasOne(u => u.SparePart)
                .WithMany(p => p.Usages)
                .HasForeignKey(u => u.SparePartId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(u => u.TaskOrder)
                .WithMany()
                .HasForeignKey(u => u.TaskOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<MaintenanceSchedule>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.Description).HasMaxLength(1000);
            e.Property(s => s.Notes).HasMaxLength(1000);
            e.Property(s => s.CreatedByUserId).IsRequired().HasMaxLength(450);
            e.HasOne(s => s.Equipment)
                .WithMany()
                .HasForeignKey(s => s.EquipmentId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.AssignedTechnician)
                .WithMany()
                .HasForeignKey(s => s.AssignedTechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.AssignedGroup)
                .WithMany()
                .HasForeignKey(s => s.AssignedGroupId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Client>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.CompanyName).HasMaxLength(300);
            e.Property(c => c.Email).IsRequired().HasMaxLength(256);
            e.Property(c => c.Phone).HasMaxLength(50);
            e.Property(c => c.Address).HasMaxLength(500);
            e.Property(c => c.Notes).HasMaxLength(1000);
        });

        builder.Entity<MaintenanceRequest>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Title).IsRequired().HasMaxLength(200);
            e.Property(r => r.Description).HasMaxLength(2000);
            e.Property(r => r.EquipmentDescription).HasMaxLength(500);
            e.Property(r => r.Notes).HasMaxLength(1000);
            e.Property(r => r.ReviewedByUserId).HasMaxLength(450);
            e.Property(r => r.ReviewNotes).HasMaxLength(2000);
            e.HasOne(r => r.Client)
                .WithMany(c => c.MaintenanceRequests)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.TaskOrder)
                .WithMany()
                .HasForeignKey(r => r.TaskOrderId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.Invoice)
                .WithMany()
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<MaintenanceRequestAssignment>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.AssignedByUserId).IsRequired().HasMaxLength(450);
            e.HasOne(a => a.MaintenanceRequest)
                .WithMany(r => r.Assignments)
                .HasForeignKey(a => a.MaintenanceRequestId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Technician)
                .WithMany()
                .HasForeignKey(a => a.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(a => new { a.MaintenanceRequestId, a.TechnicianId }).IsUnique();
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.EntityType).IsRequired().HasMaxLength(100);
            e.Property(l => l.EntityId).IsRequired().HasMaxLength(450);
            e.Property(l => l.Action).IsRequired().HasMaxLength(100);
            e.Property(l => l.PerformedByUserId).IsRequired().HasMaxLength(450);
            e.Property(l => l.PerformedByName).HasMaxLength(200);
            e.Property(l => l.Details).HasMaxLength(2000);
            e.HasIndex(l => new { l.EntityType, l.EntityId });
        });

        builder.Entity<Invoice>(e =>
        {
            e.HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PremiumService>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.Description).HasMaxLength(2000);
            e.Property(s => s.Price).HasColumnType("decimal(18,2)");
            e.Property(s => s.Features).HasMaxLength(2000);
        });

        builder.Entity<PremiumMaintenanceRequest>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Notes).HasMaxLength(1000);
            e.Property(r => r.Address).HasMaxLength(500);
            e.HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.PremiumService)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.PremiumServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Payment)
                .WithOne(p => p.PremiumMaintenanceRequest)
                .HasForeignKey<Payment>(p => p.PremiumMaintenanceRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            e.Property(p => p.TransactionId).HasMaxLength(500);
            e.Property(p => p.Notes).HasMaxLength(1000);
            e.Property(p => p.FailureReason).HasMaxLength(1000);
        });

        builder.Entity<TechnicianGpsLog>(e =>
        {
            e.HasKey(l => l.Id);
            e.HasOne(l => l.Technician)
                .WithMany()
                .HasForeignKey(l => l.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(l => new { l.TechnicianId, l.RecordedAt });
        });

        builder.Entity<Quotation>(e =>
        {
            e.HasKey(q => q.Id);
            e.Property(q => q.QuotationNumber).IsRequired().HasMaxLength(50);
            e.Property(q => q.ClientName).IsRequired().HasMaxLength(200);
            e.Property(q => q.ClientEmail).HasMaxLength(256);
            e.Property(q => q.ClientAddress).HasMaxLength(500);
            e.Property(q => q.ClientPhone).HasMaxLength(50);
            e.Property(q => q.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(q => q.TaxRate).HasColumnType("decimal(5,2)");
            e.Property(q => q.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(q => q.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(q => q.Notes).HasMaxLength(2000);
            e.Property(q => q.TermsAndConditions).HasMaxLength(4000);
            e.Property(q => q.CreatedByUserId).IsRequired().HasMaxLength(450);
            e.HasOne(q => q.MaintenanceRequest)
                .WithMany()
                .HasForeignKey(q => q.MaintenanceRequestId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(q => q.Client)
                .WithMany()
                .HasForeignKey(q => q.ClientId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasMany(q => q.LineItems)
                .WithOne(li => li.Quotation)
                .HasForeignKey(li => li.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<QuotationLineItem>(e =>
        {
            e.HasKey(li => li.Id);
            e.Property(li => li.Description).IsRequired().HasMaxLength(500);
            e.Property(li => li.Quantity).HasColumnType("decimal(10,2)");
            e.Property(li => li.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(li => li.Total).HasColumnType("decimal(18,2)");
        });
    }
}
