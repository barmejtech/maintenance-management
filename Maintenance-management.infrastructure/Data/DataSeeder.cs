using Maintenance_management.domain.Entities;
using Maintenance_management.domain.Enums;
using Maintenance_management.infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskStatus = Maintenance_management.domain.Enums.TaskStatus;

namespace Maintenance_management.infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // ── Seed roles ──────────────────────────────────────────────────────────
        string[] roles = ["Admin", "Manager", "Technician"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                    logger.LogInformation("Role '{Role}' created.", role);
                else
                    logger.LogWarning("Failed to create role '{Role}': {Errors}", role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed admin user ──────────────────────────────────────────────────────
        var adminEmail = configuration["AdminSettings:Email"] ?? "admin@maintenance.com";
        var adminPassword = configuration["AdminSettings:Password"] ?? "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                logger.LogInformation("Admin user '{Email}' created.", adminEmail);
            }
            else
            {
                logger.LogWarning("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed manager user ────────────────────────────────────────────────────
        const string managerEmail = "manager@maintenance.com";
        const string managerPassword = "Manager123!";

        if (await userManager.FindByEmailAsync(managerEmail) is null)
        {
            var manager = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                FirstName = "Maintenance",
                LastName = "Manager",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(manager, managerPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, "Manager");
                logger.LogInformation("Manager user '{Email}' created.", managerEmail);
            }
            else
            {
                logger.LogWarning("Failed to create manager user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed technician user ─────────────────────────────────────────────────
        const string techEmail = "tech@maintenance.com";
        const string techPassword = "Tech1234!";

        if (await userManager.FindByEmailAsync(techEmail) is null)
        {
            var tech = new ApplicationUser
            {
                UserName = techEmail,
                Email = techEmail,
                FirstName = "Demo",
                LastName = "Technician",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(tech, techPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(tech, "Technician");
                logger.LogInformation("Technician user '{Email}' created.", techEmail);
            }
            else
            {
                logger.LogWarning("Failed to create technician user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed domain entities ─────────────────────────────────────────────────
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var techUser    = await userManager.FindByEmailAsync(techEmail);
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        var adminUser   = await userManager.FindByEmailAsync(configuration["AdminSettings:Email"] ?? "admin@maintenance.com");
        var adminUserId = adminUser?.Id ?? "unknown";

        // ── Technicians ──────────────────────────────────────────────────────────
        var tech1Id = new Guid("11111111-1111-1111-1111-111111111111");
        var tech2Id = new Guid("22222222-2222-2222-2222-222222222222");

        if (techUser is not null && !await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech1Id,
                UserId = techUser.Id,
                FirstName = "Demo",
                LastName = "Technician",
                Email = techEmail,
                Phone = "555-0100",
                Specialization = "HVAC Systems",
                Status = TechnicianStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech1.");
        }

        if (managerUser is not null && !await context.Technicians.AnyAsync(t => t.Id == tech2Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech2Id,
                UserId = managerUser.Id,
                FirstName = "Maintenance",
                LastName = "Manager",
                Email = managerEmail,
                Phone = "555-0200",
                Specialization = "General Maintenance",
                Status = TechnicianStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech2.");
        }

        await context.SaveChangesAsync();

        // ── TechnicianGroups ─────────────────────────────────────────────────────
        var group1Id = new Guid("33333333-3333-3333-3333-333333333333");
        var group2Id = new Guid("44444444-4444-4444-4444-444444444444");

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group1Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group1Id,
                Name = "HVAC Team",
                Description = "Handles all HVAC maintenance and repairs",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group2Id,
                Name = "Electrical Team",
                Description = "Handles electrical systems and inspections",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Equipment ────────────────────────────────────────────────────────────
        var eq1Id = new Guid("55555555-5555-5555-5555-555555555555");
        var eq2Id = new Guid("66666666-6666-6666-6666-666666666666");

        if (!await context.Equipments.AnyAsync(e => e.Id == eq1Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq1Id,
                Name = "Air Handler Unit #1",
                SerialNumber = "AHU-2023-001",
                Model = "AHU-500",
                Manufacturer = "Carrier",
                Location = "Building A - Rooftop",
                InstallationDate = new DateTime(2020, 6, 15),
                LastMaintenanceDate = new DateTime(2024, 1, 10),
                NextMaintenanceDate = new DateTime(2024, 7, 10),
                Status = EquipmentStatus.Operational,
                Notes = "Primary air handler for Building A",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq2Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq2Id,
                Name = "Chiller Unit #2",
                SerialNumber = "CHU-2022-002",
                Model = "CH-800",
                Manufacturer = "Trane",
                Location = "Mechanical Room B",
                InstallationDate = new DateTime(2019, 3, 22),
                LastMaintenanceDate = new DateTime(2023, 11, 5),
                NextMaintenanceDate = new DateTime(2024, 5, 5),
                Status = EquipmentStatus.UnderMaintenance,
                Notes = "Cooling unit for floors 2-5",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── HVACSystems ──────────────────────────────────────────────────────────
        var hvac1Id = new Guid("77777777-7777-7777-7777-777777777777");
        var hvac2Id = new Guid("88888888-8888-8888-8888-888888888888");

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac1Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac1Id,
                Name = "Central AC System A",
                SystemType = "Central Air Conditioning",
                Brand = "Carrier",
                Model = "38AKS024-3",
                Capacity = 24m,
                CapacityUnit = "kW",
                RefrigerantType = "R-410A",
                Location = "Building A",
                InstallationDate = new DateTime(2020, 6, 15),
                LastInspectionDate = new DateTime(2024, 1, 10),
                NextInspectionDate = new DateTime(2024, 7, 10),
                Notes = "Main cooling system for Building A",
                EquipmentId = eq1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac2Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac2Id,
                Name = "Chiller System B",
                SystemType = "Chiller",
                Brand = "Trane",
                Model = "CGAM-025",
                Capacity = 50m,
                CapacityUnit = "Tons",
                RefrigerantType = "R-134a",
                Location = "Building B",
                InstallationDate = new DateTime(2019, 3, 22),
                LastInspectionDate = new DateTime(2023, 11, 5),
                NextInspectionDate = new DateTime(2024, 5, 5),
                Notes = "Chiller system serving floors 2-5",
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── TaskOrders ───────────────────────────────────────────────────────────
        var task1Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var task2Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task1Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task1Id,
                Title = "Quarterly HVAC Inspection",
                Description = "Perform quarterly inspection of all HVAC units in Building A",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                MaintenanceType = MaintenanceType.Inspection,
                ScheduledDate = DateTime.UtcNow.AddDays(-7),
                DueDate = DateTime.UtcNow.AddDays(7),
                Notes = "Inspect filters, belts, and refrigerant levels",
                CreatedByUserId = adminUserId,
                TechnicianId = tech1Id,
                EquipmentId = eq1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task2Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task2Id,
                Title = "Chiller Preventive Maintenance",
                Description = "Annual preventive maintenance for Chiller Unit #2",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(14),
                DueDate = DateTime.UtcNow.AddDays(30),
                Notes = "Include oil change and vibration analysis",
                CreatedByUserId = adminUserId,
                GroupId = group1Id,
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Invoices ─────────────────────────────────────────────────────────────
        var inv1Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var inv2Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");

        if (!await context.Invoices.AnyAsync(i => i.Id == inv1Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv1Id,
                InvoiceNumber = "INV-2024-001",
                ClientName = "Acme Corporation",
                ClientEmail = "billing@acme.com",
                IssueDate = DateTime.UtcNow.AddDays(-30),
                DueDate = DateTime.UtcNow,
                SubTotal = 1500m,
                TaxRate = 10m,
                TaxAmount = 150m,
                TotalAmount = 1650m,
                Status = InvoiceStatus.Sent,
                Notes = "HVAC inspection services",
                CreatedByUserId = adminUserId,
                TaskOrderId = task1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Invoices.AnyAsync(i => i.Id == inv2Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv2Id,
                InvoiceNumber = "INV-2024-002",
                ClientName = "TechCorp Inc.",
                ClientEmail = "accounts@techcorp.com",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                SubTotal = 3200m,
                TaxRate = 10m,
                TaxAmount = 320m,
                TotalAmount = 3520m,
                Status = InvoiceStatus.Draft,
                Notes = "Annual chiller maintenance",
                CreatedByUserId = adminUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── InvoiceLineItems ─────────────────────────────────────────────────────
        var li1Id = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var li2Id = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var li3Id = new Guid("00000000-0000-0000-0001-000000000001");

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li1Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li1Id,
                Description = "HVAC Inspection Labor",
                Quantity = 8m,
                UnitPrice = 150m,
                Total = 1200m,
                InvoiceId = inv1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li2Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li2Id,
                Description = "Filter Replacement Parts",
                Quantity = 3m,
                UnitPrice = 100m,
                Total = 300m,
                InvoiceId = inv1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li3Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li3Id,
                Description = "Chiller Preventive Maintenance",
                Quantity = 16m,
                UnitPrice = 200m,
                Total = 3200m,
                InvoiceId = inv2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceReports ───────────────────────────────────────────────────
        var report1Id = new Guid("00000000-0000-0000-0002-000000000002");
        var report2Id = new Guid("00000000-0000-0000-0003-000000000003");
        var techFullName = techUser is not null ? "Demo Technician" : "Unknown Technician";

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report1Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report1Id,
                Title = "HVAC Q1 Inspection Report",
                Content = "Completed quarterly HVAC inspection of Building A. All filters replaced. Refrigerant levels are within acceptable range. Belt tension adjusted on AHU #1. No major issues found.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-5),
                LaborHours = 8m,
                MaterialCost = 300m,
                Recommendations = "Schedule annual deep cleaning for next quarter.",
                TaskOrderId = task1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report2Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report2Id,
                Title = "Chiller System Assessment",
                Content = "Pre-maintenance assessment of Chiller Unit #2. Found minor oil leak at pump seal. Vibration levels slightly elevated. Unit still operational but preventive maintenance is critical.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-2),
                LaborHours = 4m,
                MaterialCost = 50m,
                Recommendations = "Replace pump seal during scheduled maintenance. Order R-134a refrigerant.",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Availability ─────────────────────────────────────────────────────────
        var avail1Id = new Guid("00000000-0000-0000-0004-000000000004");
        var avail2Id = new Guid("00000000-0000-0000-0005-000000000005");

        if (!await context.Availabilities.AnyAsync(a => a.Id == avail1Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.Availabilities.Add(new Availability
            {
                Id = avail1Id,
                TechnicianId = tech1Id,
                StartTime = DateTime.UtcNow.Date.AddHours(8),
                EndTime = DateTime.UtcNow.Date.AddHours(17),
                IsAvailable = true,
                Notes = "Regular work hours",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Availabilities.AnyAsync(a => a.Id == avail2Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech2Id))
        {
            context.Availabilities.Add(new Availability
            {
                Id = avail2Id,
                TechnicianId = tech2Id,
                StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
                EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(18),
                IsAvailable = false,
                Notes = "Scheduled day off",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Domain entity seeding complete.");
    }
}
