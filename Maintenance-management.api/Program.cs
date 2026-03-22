using Maintenance_management.domain.Interfaces;
using Maintenance_management.infrastructure.Data;
using Maintenance_management.infrastructure.Identity;
using Maintenance_management.infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

// ===================== SERVICES =====================

// Add SignalR services
builder.Services.AddSignalR();


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

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(swaggerSchemeId)] = []
    });
});

var app = builder.Build();

// ===================== DATABASE SEEDING =====================
using (var scope = app.Services.CreateScope())
{
    try
    {
       // await DataSeeder.SeedAsync(scope.ServiceProvider);
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
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notifications").RequireCors("AllowSignalR");
app.MapHub<ChatHub>("/hubs/chat").RequireCors("AllowSignalR");

app.MapControllers();

app.Run();