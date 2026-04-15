using Maintenance_management.api.Hubs;
using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Maintenance_management.infrastructure.Identity;
using Maintenance_management.infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("JwtSettings");

// ===================== DATABASE =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);

// ===================== IDENTITY =====================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===================== JWT AUTH =====================
var jwtSecurityKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = jwtSecurityKey,
        ClockSkew = TimeSpan.Zero
    };

    // Allow SignalR clients to pass the JWT token via the query string
    // (WebSocket and Server-Sent Events connections cannot set HTTP headers)
    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ===================== CORS =====================
builder.Services.AddCors(options =>
{
    // Development: allow any origin (cannot be combined with AllowCredentials)
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

    // Production-ready policy for SignalR: restrict to known origins and allow credentials
    options.AddPolicy("AllowSignalR", policy =>
        policy.WithOrigins(
                  builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                  ?? new[] { "http://localhost:3000", "http://localhost:4200" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ===================== REPOSITORIES =====================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IManagerRepository, Maintenance_management.infrastructure.Repositories.ManagerRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IDataEntryRepository, Maintenance_management.infrastructure.Repositories.DataEntryRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.ITechnicianRepository, Maintenance_management.infrastructure.Repositories.TechnicianRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.ITaskOrderRepository, Maintenance_management.infrastructure.Repositories.TaskOrderRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IEquipmentRepository, Maintenance_management.infrastructure.Repositories.EquipmentRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IGroupRepository, Maintenance_management.infrastructure.Repositories.GroupRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IHVACSystemRepository, Maintenance_management.infrastructure.Repositories.HVACSystemRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IInvoiceRepository, Maintenance_management.infrastructure.Repositories.InvoiceRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IReportRepository, Maintenance_management.infrastructure.Repositories.ReportRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IAvailabilityRepository, Maintenance_management.infrastructure.Repositories.AvailabilityRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IDocumentRepository, Maintenance_management.infrastructure.Repositories.DocumentRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IChatMessageRepository, Maintenance_management.infrastructure.Repositories.ChatMessageRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.INotificationRepository, Maintenance_management.infrastructure.Repositories.NotificationRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.ITechnicianPerformanceScoreRepository, Maintenance_management.infrastructure.Repositories.TechnicianPerformanceScoreRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.ISparePartRepository, Maintenance_management.infrastructure.Repositories.SparePartRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IMaintenanceScheduleRepository, Maintenance_management.infrastructure.Repositories.MaintenanceScheduleRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IClientRepository, Maintenance_management.infrastructure.Repositories.ClientRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IMaintenanceRequestRepository, Maintenance_management.infrastructure.Repositories.MaintenanceRequestRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IAuditLogRepository, Maintenance_management.infrastructure.Repositories.AuditLogRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IPremiumServiceRepository, Maintenance_management.infrastructure.Repositories.PremiumServiceRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IPremiumMaintenanceRequestRepository, Maintenance_management.infrastructure.Repositories.PremiumMaintenanceRequestRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IPaymentRepository, Maintenance_management.infrastructure.Repositories.PaymentRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.ITechnicianGpsLogRepository, Maintenance_management.infrastructure.Repositories.TechnicianGpsLogRepository>();
builder.Services.AddScoped<Maintenance_management.domain.Interfaces.IQuotationRepository, Maintenance_management.infrastructure.Repositories.QuotationRepository>();

// Application services
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IAuthService, Maintenance_management.infrastructure.Services.AuthService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IEmailService, Maintenance_management.infrastructure.Services.EmailService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IJwtService, Maintenance_management.infrastructure.Services.JwtService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IIdentityService, Maintenance_management.infrastructure.Services.IdentityService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IManagerService, Maintenance_management.application.Services.ManagerService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IDataEntryService, Maintenance_management.application.Services.DataEntryService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ITechnicianService, Maintenance_management.application.Services.TechnicianService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ITaskOrderService, Maintenance_management.application.Services.TaskOrderService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IGroupService, Maintenance_management.application.Services.GroupService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IEquipmentService, Maintenance_management.application.Services.EquipmentService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IHVACService, Maintenance_management.application.Services.HVACService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IReportService, Maintenance_management.application.Services.ReportService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IInvoiceService, Maintenance_management.application.Services.InvoiceService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IAvailabilityService, Maintenance_management.application.Services.AvailabilityService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ITechnicianPerformanceService, Maintenance_management.application.Services.TechnicianPerformanceService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ISparePartService, Maintenance_management.application.Services.SparePartService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IMaintenanceScheduleService, Maintenance_management.application.Services.MaintenanceScheduleService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IUserService, Maintenance_management.application.Services.UserService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IAccountService, Maintenance_management.infrastructure.Services.AccountService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IClientService, Maintenance_management.application.Services.ClientService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IMaintenanceRequestService, Maintenance_management.application.Services.MaintenanceRequestService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IPremiumServiceService, Maintenance_management.application.Services.PremiumServiceService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IPremiumMaintenanceRequestService, Maintenance_management.application.Services.PremiumMaintenanceRequestService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IPaymentService, Maintenance_management.application.Services.PaymentService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IAdminDashboardService, Maintenance_management.application.Services.AdminDashboardService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ITravelEstimationService, Maintenance_management.api.Services.TravelEstimationService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.ISmsService, Maintenance_management.infrastructure.Services.SmsService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IGpsTrackingService, Maintenance_management.application.Services.GpsTrackingService>();
builder.Services.AddScoped<Maintenance_management.application.Interfaces.IQuotationService, Maintenance_management.application.Services.QuotationService>();

// Notification service (SignalR-based)
builder.Services.AddScoped<Maintenance_management.application.Interfaces.INotificationService, Maintenance_management.api.Services.HubNotificationService>();

// Background service: sends notifications for maintenance schedules due today
builder.Services.AddHostedService<Maintenance_management.api.Services.ScheduleTodayNotificationService>();

// Background service: checks technician GPS reporting every 4 hours
builder.Services.AddHostedService<Maintenance_management.api.Services.GpsPollingBackgroundService>();

// ===================== SERVICES =====================

// Add SignalR services
builder.Services.AddSignalR();

// HTTP client for external APIs (geocoding + routing)
builder.Services.AddHttpClient("TravelEstimation", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MaintenanceManagement/1.0");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// In-memory cache for travel estimates
builder.Services.AddMemoryCache();


//builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===================== CONTROLLERS =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===================== SWAGGER =====================
const string swaggerSchemeId = "Bearer";

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Municipal Licensing System API",
        Version = "v1"
    });

    // ✅ Swashbuckle v10 compatible security definition
    options.AddSecurityDefinition(swaggerSchemeId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token below.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",        // must be lowercase
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(swaggerSchemeId, doc)] = []
    });
});

var app = builder.Build();

// ===================== DATABASE SEEDING =====================
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DataSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// ===================== PIPELINE =====================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Municipal Licensing System API v1");
    c.RoutePrefix = string.Empty;
});

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notifications").RequireCors("AllowSignalR");
app.MapHub<ChatHub>("/hubs/chat").RequireCors("AllowSignalR");

app.MapControllers();

app.Run();