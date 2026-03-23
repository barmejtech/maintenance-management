using Maintenance_management.domain.Entities;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Maintenance_management.infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            e.Property(m => m.RecipientId).HasMaxLength(450);
            e.Property(m => m.Content).HasMaxLength(4000);
            e.Property(m => m.FileUrl).HasMaxLength(1000);
            e.Property(m => m.FileName).HasMaxLength(500);
            e.Property(m => m.ContentType).HasMaxLength(100);
        });
    }
}
