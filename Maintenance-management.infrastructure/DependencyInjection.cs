using Maintenance_management.application.Interfaces;
using Maintenance_management.application.Services;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Maintenance_management.infrastructure.Identity;
using Maintenance_management.infrastructure.Repositories;
using Maintenance_management.infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maintenance_management.infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core + SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ASP.NET Core Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IManagerRepository, ManagerRepository>();
        services.AddScoped<IDataEntryRepository, DataEntryRepository>();
        services.AddScoped<ITechnicianRepository, TechnicianRepository>();
        services.AddScoped<ITaskOrderRepository, TaskOrderRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IHVACSystemRepository, HVACSystemRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEquipmentHealthPredictionRepository, EquipmentHealthPredictionRepository>();
        services.AddScoped<IEquipmentDigitalTwinRepository, EquipmentDigitalTwinRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IManagerService, ManagerService>();
        services.AddScoped<IDataEntryService, DataEntryService>();
        services.AddScoped<ITechnicianService, TechnicianService>();
        services.AddScoped<ITaskOrderService, TaskOrderService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IHVACService, HVACService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IPredictiveMaintenanceService, PredictiveMaintenanceService>();
        services.AddScoped<IEquipmentDigitalTwinService, EquipmentDigitalTwinService>();

        return services;
    }
}
