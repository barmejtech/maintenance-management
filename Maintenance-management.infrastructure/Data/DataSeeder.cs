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
        string[] roles = ["Admin", "Manager", "Technician", "DataEntry", "Client", "Support"];
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
                FirstName = "Wei Ming",
                LastName = "Tan",
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
                FirstName = "Jia Hui",
                LastName = "Lim",
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
                FirstName = "Raj",
                LastName = "Kumar",
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

        // ── Seed data-entry user ─────────────────────────────────────────────────
        const string dataEntryEmail = "dataentry@maintenance.com";
        const string dataEntryPassword = "DataEntry123!";

        if (await userManager.FindByEmailAsync(dataEntryEmail) is null)
        {
            var dataEntry = new ApplicationUser
            {
                UserName = dataEntryEmail,
                Email = dataEntryEmail,
                FirstName = "Mei Lin",
                LastName = "Wong",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(dataEntry, dataEntryPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(dataEntry, "DataEntry");
                logger.LogInformation("DataEntry user '{Email}' created.", dataEntryEmail);
            }
            else
            {
                logger.LogWarning("Failed to create data-entry user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed support user ────────────────────────────────────────────────────
        const string supportEmail = "support@maintenance.com";
        const string supportPassword = "Support@123";

        if (await userManager.FindByEmailAsync(supportEmail) is null)
        {
            var support = new ApplicationUser
            {
                UserName = supportEmail,
                Email = supportEmail,
                FirstName = "Kavin",
                LastName = "Nair",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(support, supportPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(support, "Support");
                logger.LogInformation("Support user '{Email}' created.", supportEmail);
            }
            else
            {
                logger.LogWarning("Failed to create support user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed second technician user ───────────────────────────────────────────
        const string tech2Email = "tech2@maintenance.my";
        const string tech2Password = "Tech1234!";

        if (await userManager.FindByEmailAsync(tech2Email) is null)
        {
            var tech2 = new ApplicationUser
            {
                UserName = tech2Email,
                Email = tech2Email,
                FirstName = "Arjun",
                LastName = "Tan",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(tech2, tech2Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(tech2, "Technician");
                logger.LogInformation("Technician2 user '{Email}' created.", tech2Email);
            }
            else
            {
                logger.LogWarning("Failed to create technician2 user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed third technician user ────────────────────────────────────────────
        const string tech3Email = "tech3@maintenance.my";
        const string tech3Password = "Tech1234!";

        if (await userManager.FindByEmailAsync(tech3Email) is null)
        {
            var tech3 = new ApplicationUser
            {
                UserName = tech3Email,
                Email = tech3Email,
                FirstName = "Kok Beng",
                LastName = "Lee",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(tech3, tech3Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(tech3, "Technician");
                logger.LogInformation("Technician3 user '{Email}' created.", tech3Email);
            }
            else
            {
                logger.LogWarning("Failed to create technician3 user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed second manager user ──────────────────────────────────────────────
        const string manager2Email = "manager2@maintenance.my";
        const string manager2Password = "Manager123!";

        if (await userManager.FindByEmailAsync(manager2Email) is null)
        {
            var manager2 = new ApplicationUser
            {
                UserName = manager2Email,
                Email = manager2Email,
                FirstName = "Pei Shan",
                LastName = "Low",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(manager2, manager2Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager2, "Manager");
                logger.LogInformation("Manager2 user '{Email}' created.", manager2Email);
            }
            else
            {
                logger.LogWarning("Failed to create manager2 user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Seed second data-entry user ───────────────────────────────────────────
        const string dataEntry2Email = "dataentry2@maintenance.my";
        const string dataEntry2Password = "DataEntry123!";

        if (await userManager.FindByEmailAsync(dataEntry2Email) is null)
        {
            var dataEntry2 = new ApplicationUser
            {
                UserName = dataEntry2Email,
                Email = dataEntry2Email,
                FirstName = "Viknesh",
                LastName = "Menon",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(dataEntry2, dataEntry2Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(dataEntry2, "DataEntry");
                logger.LogInformation("DataEntry2 user '{Email}' created.", dataEntry2Email);
            }
            else
            {
                logger.LogWarning("Failed to create dataEntry2 user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var techUser = await userManager.FindByEmailAsync(techEmail);
        var managerUser = await userManager.FindByEmailAsync(managerEmail);
        var dataEntryUser = await userManager.FindByEmailAsync(dataEntryEmail);
        var tech2User = await userManager.FindByEmailAsync(tech2Email);
        var tech3User = await userManager.FindByEmailAsync(tech3Email);
        var manager2User = await userManager.FindByEmailAsync(manager2Email);
        var dataEntry2User = await userManager.FindByEmailAsync(dataEntry2Email);
        var adminUser = await userManager.FindByEmailAsync(configuration["AdminSettings:Email"] ?? "admin@maintenance.com");
        if (adminUser is null)
        {
            logger.LogWarning("Admin user not found; skipping domain entity seeding.");
            return;
        }
        var adminUserId = adminUser.Id;

        // ── Property Management Core (UnitTypes / Units / Owners / Tenants) ─────
        var unitTypeSeeds = new List<UnitType>
        {
            new()
            {
                Id = new Guid("10000000-0000-0000-0000-000000000001"),
                Name = "Studio",
                Description = "وحدة استوديو مناسبة للأفراد | Studio unit suitable for singles",
                DefaultSizeSqm = 42m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new Guid("10000000-0000-0000-0000-000000000002"),
                Name = "1BR",
                Description = "شقة غرفة وصالة | One-bedroom apartment",
                DefaultSizeSqm = 68m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new Guid("10000000-0000-0000-0000-000000000003"),
                Name = "2BR",
                Description = "شقة غرفتين وصالة | Two-bedroom apartment",
                DefaultSizeSqm = 102m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new Guid("10000000-0000-0000-0000-000000000004"),
                Name = "3BR",
                Description = "شقة ثلاث غرف وصالة | Three-bedroom apartment",
                DefaultSizeSqm = 138m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new Guid("10000000-0000-0000-0000-000000000005"),
                Name = "Penthouse",
                Description = "بنتهاوس فاخر بإطلالة كاملة | Luxury penthouse with panoramic view",
                DefaultSizeSqm = 245m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var unitSeeds = new List<Unit>
        {
            new() { Id = new Guid("10000000-0000-0000-0001-000000000001"), UnitNumber = "A-101", Floor = 1, SizeSqm = 41m, Status = UnitStatus.Occupied, ShareValue = 0.92m, UnitTypeId = unitTypeSeeds[0].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000002"), UnitNumber = "A-102", Floor = 1, SizeSqm = 43m, Status = UnitStatus.Vacant, ShareValue = 0.95m, UnitTypeId = unitTypeSeeds[0].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000003"), UnitNumber = "A-103", Floor = 1, SizeSqm = 44m, Status = UnitStatus.Occupied, ShareValue = 0.97m, UnitTypeId = unitTypeSeeds[0].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000004"), UnitNumber = "A-201", Floor = 2, SizeSqm = 67m, Status = UnitStatus.Occupied, ShareValue = 1.11m, UnitTypeId = unitTypeSeeds[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000005"), UnitNumber = "A-202", Floor = 2, SizeSqm = 69m, Status = UnitStatus.UnderMaintenance, ShareValue = 1.13m, UnitTypeId = unitTypeSeeds[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000006"), UnitNumber = "A-203", Floor = 2, SizeSqm = 70m, Status = UnitStatus.Occupied, ShareValue = 1.14m, UnitTypeId = unitTypeSeeds[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000007"), UnitNumber = "B-301", Floor = 3, SizeSqm = 101m, Status = UnitStatus.Occupied, ShareValue = 1.41m, UnitTypeId = unitTypeSeeds[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000008"), UnitNumber = "B-302", Floor = 3, SizeSqm = 103m, Status = UnitStatus.Occupied, ShareValue = 1.43m, UnitTypeId = unitTypeSeeds[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000009"), UnitNumber = "B-303", Floor = 3, SizeSqm = 105m, Status = UnitStatus.Vacant, ShareValue = 1.45m, UnitTypeId = unitTypeSeeds[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000010"), UnitNumber = "B-304", Floor = 3, SizeSqm = 106m, Status = UnitStatus.Occupied, ShareValue = 1.47m, UnitTypeId = unitTypeSeeds[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000011"), UnitNumber = "C-401", Floor = 4, SizeSqm = 137m, Status = UnitStatus.Occupied, ShareValue = 1.79m, UnitTypeId = unitTypeSeeds[3].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000012"), UnitNumber = "C-402", Floor = 4, SizeSqm = 139m, Status = UnitStatus.Occupied, ShareValue = 1.82m, UnitTypeId = unitTypeSeeds[3].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000013"), UnitNumber = "C-403", Floor = 4, SizeSqm = 141m, Status = UnitStatus.Reserved, ShareValue = 1.85m, UnitTypeId = unitTypeSeeds[3].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000014"), UnitNumber = "C-404", Floor = 4, SizeSqm = 142m, Status = UnitStatus.Occupied, ShareValue = 1.87m, UnitTypeId = unitTypeSeeds[3].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000015"), UnitNumber = "D-501", Floor = 5, SizeSqm = 245m, Status = UnitStatus.Occupied, ShareValue = 2.33m, UnitTypeId = unitTypeSeeds[4].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000016"), UnitNumber = "D-502", Floor = 5, SizeSqm = 247m, Status = UnitStatus.Occupied, ShareValue = 2.36m, UnitTypeId = unitTypeSeeds[4].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000017"), UnitNumber = "D-503", Floor = 5, SizeSqm = 249m, Status = UnitStatus.Vacant, ShareValue = 2.39m, UnitTypeId = unitTypeSeeds[4].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000018"), UnitNumber = "E-601", Floor = 6, SizeSqm = 102m, Status = UnitStatus.Occupied, ShareValue = 1.46m, UnitTypeId = unitTypeSeeds[2].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000019"), UnitNumber = "E-602", Floor = 6, SizeSqm = 138m, Status = UnitStatus.Occupied, ShareValue = 1.83m, UnitTypeId = unitTypeSeeds[3].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0001-000000000020"), UnitNumber = "E-603", Floor = 6, SizeSqm = 69m, Status = UnitStatus.Occupied, ShareValue = 1.12m, UnitTypeId = unitTypeSeeds[1].Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var ownerSeeds = new List<Owner>
        {
            new() { Id = new Guid("10000000-0000-0000-0002-000000000001"), FullName = "Daniel Iskandar Nair", Email = "daniel.nair@property.my", Phone = "+60101110101", Address = "دامنسارا هايتس، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000002"), FullName = "Mei Ling Viknesh Lim", Email = "meiling.lim@property.my", Phone = "+60101110102", Address = "تامان تون، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000003"), FullName = "Adrian Low", Email = "adrian.low@property.my", Phone = "+60101110103", Address = "بندر سونواي، شاه عالم", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000004"), FullName = "Pei Shan Wei Ming Tan", Email = "peishan.tan@property.my", Phone = "+60101110104", Address = "سري هارتاماس، بوتراجايا", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000005"), FullName = "Viknesh Suren Wong", Email = "viknesh.wong@property.my", Phone = "+60101110105", Address = "مونت كيارا، إيبوه", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000006"), FullName = "Kavitha Kumar", Email = "kavitha.kumar@property.my", Phone = "+60101110106", Address = "بانغسار، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000007"), FullName = "Shawn Jia Hui Lee", Email = "shawn.lee@property.my", Phone = "+60101110107", Address = "تامان موليا، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000008"), FullName = "Sabrina Teh", Email = "sabrina.teh@property.my", Phone = "+60101110108", Address = "كوتا دامنسارا، شاه عالم", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000009"), FullName = "Marcus Kelvin Ong", Email = "marcus.ong@property.my", Phone = "+60101110109", Address = "ديزا بارك سيتي، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000010"), FullName = "Carmen Nair", Email = "carmen.nair@property.my", Phone = "+60101110110", Address = "تامان سيبوتيه، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000011"), FullName = "Brandon Pravin Cheah", Email = "brandon.cheah@property.my", Phone = "+60101110111", Address = "سكشن 13، شاه عالم", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000012"), FullName = "Cheryl Lim", Email = "cheryl.lim@property.my", Phone = "+60101110112", Address = "سيتي وانغسا، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000013"), FullName = "Darren Teh", Email = "darren.teh@property.my", Phone = "+60101110113", Address = "تامان بيلاڠي، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000014"), FullName = "Ashwin Tan", Email = "ashwin.tan@property.my", Phone = "+60101110114", Address = "جورج تاون سنترال، جورج تاون", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000015"), FullName = "Pei Yee Raj Low", Email = "peiyee.low@property.my", Phone = "+60101110115", Address = "كيارا باي، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000016"), FullName = "Ian Menon", Email = "ian.menon@property.my", Phone = "+60101110116", Address = "تامان داناو ديسا، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000017"), FullName = "Kavitha Jerome Krishnan", Email = "kavitha.krishnan@property.my", Phone = "+60101110117", Address = "غورني درايف، جورج تاون", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000018"), FullName = "Raj Wong", Email = "raj.wong@property.my", Phone = "+60101110118", Address = "تامان ديسا، كوالالمبور", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000019"), FullName = "Nisha Jia Hui Nair", Email = "nisha.nair@property.my", Phone = "+60101110119", Address = "تامان سنتوسا، جوهور باهرو", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000020"), FullName = "Brandon Lee", Email = "brandon.lee@property.my", Phone = "+60101110120", Address = "بوكيت جليل، شاه عالم", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000021"), FullName = "Alicia Ramli", Email = "alicia.ramli@property.my", Phone = "+60101110121", Address = "بريسينكت 8، بوتراجايا", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0002-000000000022"), FullName = "Marcus Lim", Email = "marcus.lim@property.my", Phone = "+60101110122", Address = "غريينتاون، إيبوه", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var ownershipSeeds = new List<UnitOwnership>
        {
            new() { Id = new Guid("10000000-0000-0000-0003-000000000001"), UnitId = unitSeeds[0].Id, OwnerId = ownerSeeds[0].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 1, 5), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000002"), UnitId = unitSeeds[1].Id, OwnerId = ownerSeeds[1].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 3, 11), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000003"), UnitId = unitSeeds[2].Id, OwnerId = ownerSeeds[2].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2019, 8, 24), SaleDate = new DateTime(2024, 2, 15), IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000004"), UnitId = unitSeeds[2].Id, OwnerId = ownerSeeds[3].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2024, 2, 16), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000005"), UnitId = unitSeeds[3].Id, OwnerId = ownerSeeds[4].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2021, 6, 20), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000006"), UnitId = unitSeeds[4].Id, OwnerId = ownerSeeds[5].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2023, 9, 10), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000007"), UnitId = unitSeeds[5].Id, OwnerId = ownerSeeds[6].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2021, 11, 12), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000008"), UnitId = unitSeeds[6].Id, OwnerId = ownerSeeds[7].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 5, 8), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000009"), UnitId = unitSeeds[7].Id, OwnerId = ownerSeeds[8].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2022, 4, 19), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000010"), UnitId = unitSeeds[8].Id, OwnerId = ownerSeeds[9].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2023, 2, 22), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000011"), UnitId = unitSeeds[9].Id, OwnerId = ownerSeeds[10].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 7, 4), SaleDate = new DateTime(2025, 1, 5), IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000012"), UnitId = unitSeeds[9].Id, OwnerId = ownerSeeds[11].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2025, 1, 6), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000013"), UnitId = unitSeeds[10].Id, OwnerId = ownerSeeds[12].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2019, 12, 30), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000014"), UnitId = unitSeeds[11].Id, OwnerId = ownerSeeds[13].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2022, 8, 13), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000015"), UnitId = unitSeeds[12].Id, OwnerId = ownerSeeds[14].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2023, 10, 23), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000016"), UnitId = unitSeeds[13].Id, OwnerId = ownerSeeds[15].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2021, 1, 17), SaleDate = new DateTime(2024, 9, 30), IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000017"), UnitId = unitSeeds[13].Id, OwnerId = ownerSeeds[16].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2024, 10, 1), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000018"), UnitId = unitSeeds[14].Id, OwnerId = ownerSeeds[17].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 10, 9), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000019"), UnitId = unitSeeds[15].Id, OwnerId = ownerSeeds[18].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2021, 9, 3), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000020"), UnitId = unitSeeds[16].Id, OwnerId = ownerSeeds[19].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2025, 3, 27), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000021"), UnitId = unitSeeds[17].Id, OwnerId = ownerSeeds[20].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2022, 2, 14), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000022"), UnitId = unitSeeds[18].Id, OwnerId = ownerSeeds[21].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2021, 4, 2), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0003-000000000023"), UnitId = unitSeeds[19].Id, OwnerId = ownerSeeds[0].Id, OwnershipPercentage = 100m, PurchaseDate = new DateTime(2020, 6, 18), IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var tenantSeeds = new List<Tenant>
        {
            new() { Id = new Guid("10000000-0000-0000-0004-000000000001"), UnitId = unitSeeds[0].Id, FullName = "Arjun Goh", Email = "tenant.arjun1@email.my", Phone = "+60111000201", EmergencyContactName = "Pei Ling Goh", EmergencyContactPhone = "+60111900201", LeaseStartDate = new DateTime(2025, 1, 1), LeaseEndDate = new DateTime(2025, 12, 31), RentalAmount = 3200m, DepositAmount = 3200m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000002"), UnitId = unitSeeds[3].Id, FullName = "Pei Yee Ganesan Kumar", Email = "tenant.peiyee2@email.my", Phone = "+60111000202", EmergencyContactName = "Ganesan Kumar", EmergencyContactPhone = "+60111900202", LeaseStartDate = new DateTime(2025, 2, 1), LeaseEndDate = new DateTime(2026, 1, 31), RentalAmount = 4500m, DepositAmount = 4500m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000003"), UnitId = unitSeeds[5].Id, FullName = "Priya Pillai", Email = "tenant.priya3@email.my", Phone = "+60111000203", EmergencyContactName = "Raj Pillai", EmergencyContactPhone = "+60111900203", LeaseStartDate = new DateTime(2025, 1, 15), LeaseEndDate = new DateTime(2026, 1, 14), RentalAmount = 4700m, DepositAmount = 4700m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000004"), UnitId = unitSeeds[6].Id, FullName = "Pravin Wei Ming Lee", Email = "tenant.pravin4@email.my", Phone = "+60111000204", EmergencyContactName = "Wei Ming Lee", EmergencyContactPhone = "+60111900204", LeaseStartDate = new DateTime(2025, 3, 1), LeaseEndDate = new DateTime(2026, 2, 28), RentalAmount = 6200m, DepositAmount = 6200m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000005"), UnitId = unitSeeds[7].Id, FullName = "Hannah Lim", Email = "tenant.hannah5@email.my", Phone = "+60111000205", EmergencyContactName = "Kok Beng Lim", EmergencyContactPhone = "+60111900205", LeaseStartDate = new DateTime(2025, 4, 1), LeaseEndDate = new DateTime(2026, 3, 31), RentalAmount = 6400m, DepositAmount = 6400m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000006"), UnitId = unitSeeds[9].Id, FullName = "Brandon Chan", Email = "tenant.brandon6@email.my", Phone = "+60111000206", EmergencyContactName = "Siew Ling Chan", EmergencyContactPhone = "+60111900206", LeaseStartDate = new DateTime(2025, 2, 20), LeaseEndDate = new DateTime(2026, 2, 19), RentalAmount = 6600m, DepositAmount = 6600m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000007"), UnitId = unitSeeds[10].Id, FullName = "Jason Ong", Email = "tenant.jason7@email.my", Phone = "+60111000207", EmergencyContactName = "Marcus Ong", EmergencyContactPhone = "+60111900207", LeaseStartDate = new DateTime(2025, 1, 10), LeaseEndDate = new DateTime(2026, 1, 9), RentalAmount = 7900m, DepositAmount = 7900m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000008"), UnitId = unitSeeds[11].Id, FullName = "Anjali Jia Hui Low", Email = "tenant.anjali8@email.my", Phone = "+60111000208", EmergencyContactName = "Jia Hui Low", EmergencyContactPhone = "+60111900208", LeaseStartDate = new DateTime(2025, 2, 5), LeaseEndDate = new DateTime(2026, 2, 4), RentalAmount = 8100m, DepositAmount = 8100m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000009"), UnitId = unitSeeds[14].Id, FullName = "Vanessa Tan", Email = "tenant.vanessa9@email.my", Phone = "+60111000209", EmergencyContactName = "Kelvin Tan", EmergencyContactPhone = "+60111900209", LeaseStartDate = new DateTime(2025, 1, 1), LeaseEndDate = new DateTime(2025, 12, 31), RentalAmount = 13500m, DepositAmount = 13500m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000010"), UnitId = unitSeeds[15].Id, FullName = "Wei Ming Darren Kumar", Email = "tenant.weiming10@email.my", Phone = "+60111000210", EmergencyContactName = "Darren Kumar", EmergencyContactPhone = "+60111900210", LeaseStartDate = new DateTime(2025, 3, 12), LeaseEndDate = new DateTime(2026, 3, 11), RentalAmount = 13800m, DepositAmount = 13800m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000011"), UnitId = unitSeeds[18].Id, FullName = "Trisha Lee", Email = "tenant.trisha11@email.my", Phone = "+60111000211", EmergencyContactName = "Kavitha Lee", EmergencyContactPhone = "+60111900211", LeaseStartDate = new DateTime(2025, 4, 10), LeaseEndDate = new DateTime(2026, 4, 9), RentalAmount = 8600m, DepositAmount = 8600m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = new Guid("10000000-0000-0000-0004-000000000012"), UnitId = unitSeeds[19].Id, FullName = "Ashwin Daniel Wong", Email = "tenant.ashwin12@email.my", Phone = "+60111000212", EmergencyContactName = "Daniel Wong", EmergencyContactPhone = "+60111900212", LeaseStartDate = new DateTime(2025, 1, 25), LeaseEndDate = new DateTime(2026, 1, 24), RentalAmount = 4900m, DepositAmount = 4900m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var seededUnitTypes = 0;
        foreach (var unitType in unitTypeSeeds)
        {
            if (!await context.UnitTypes.AnyAsync(ut => ut.Id == unitType.Id))
            {
                context.UnitTypes.Add(unitType);
                seededUnitTypes++;
            }
        }

        var seededUnits = 0;
        foreach (var unit in unitSeeds)
        {
            if (!await context.Units.AnyAsync(u => u.Id == unit.Id))
            {
                context.Units.Add(unit);
                seededUnits++;
            }
        }

        var seededOwners = 0;
        foreach (var owner in ownerSeeds)
        {
            if (!await context.Owners.AnyAsync(o => o.Id == owner.Id))
            {
                context.Owners.Add(owner);
                seededOwners++;
            }
        }

        await context.SaveChangesAsync();

        var seededOwnerships = 0;
        foreach (var ownership in ownershipSeeds)
        {
            if (!await context.UnitOwnerships.AnyAsync(uo => uo.Id == ownership.Id))
            {
                context.UnitOwnerships.Add(ownership);
                seededOwnerships++;
            }
        }

        var seededTenants = 0;
        foreach (var tenant in tenantSeeds)
        {
            if (!await context.Tenants.AnyAsync(t => t.Id == tenant.Id))
            {
                context.Tenants.Add(tenant);
                seededTenants++;
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation(
            "Property management seed complete. Added UnitTypes={UnitTypes}, Units={Units}, Owners={Owners}, OwnershipHistory={Ownerships}, Tenants={Tenants}.",
            seededUnitTypes, seededUnits, seededOwners, seededOwnerships, seededTenants);

        // ── Technicians ──────────────────────────────────────────────────────────
        var tech1Id = new Guid("11111111-1111-1111-1111-111111111111");
        var tech2Id = new Guid("22222222-2222-2222-2222-222222222222");

        if (techUser is not null && !await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech1Id,
                UserId = techUser.Id,
                FirstName = "Raj",
                LastName = "Kumar",
                Email = techEmail,
                Phone = "+60 11 123 4567",
                Specialization = "ميكانيكا السيارات وتشخيص الأعطال",
                Address = "تامان ملوري، كوالالمبور",
                Latitude = 3.1478,
                Longitude = 101.7133,
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
                FirstName = "Jia Hui",
                LastName = "Lim",
                Email = managerEmail,
                Phone = "+60 11 234 5678",
                Specialization = "الصيانة العامة",
                Address = "بوكيت بينتانغ، كوالالمبور",
                Latitude = 3.1390,
                Longitude = 101.6869,
                Status = TechnicianStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech2.");
        }

        var tech3Id = new Guid("00000000-0000-0000-0060-000000000060");
        var tech4Id = new Guid("00000000-0000-0000-0061-000000000061");

        if (tech2User is not null && !await context.Technicians.AnyAsync(t => t.Id == tech3Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech3Id,
                UserId = tech2User.Id,
                FirstName = "Arjun",
                LastName = "Tan",
                Email = tech2Email,
                Phone = "+60 10 678 9012",
                Specialization = "صيانة المحركات وناقل الحركة",
                Address = "تامان تون، جوهور باهرو 80250",
                Latitude = 1.4927,
                Longitude = 103.7414,
                Status = TechnicianStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech3 (Johor Bahru).");
        }

        if (tech3User is not null && !await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech4Id,
                UserId = tech3User.Id,
                FirstName = "Kok Beng",
                LastName = "Lee",
                Email = tech3Email,
                Phone = "+60 12 789 0123",
                Specialization = "كهرباء السيارات والبطاريات",
                Address = "سكشن 13، شاه عالم 40100",
                Latitude = 3.0738,
                Longitude = 101.5183,
                Status = TechnicianStatus.Busy,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech4 (Shah Alam).");
        }

        await context.SaveChangesAsync();

        // ── Managers ─────────────────────────────────────────────────────────────
        var manager1Id = new Guid("00000000-0000-0000-0040-000000000040");
        var manager2Id = new Guid("00000000-0000-0000-0091-000000000091");

        if (managerUser is not null && !await context.Managers.AnyAsync(m => m.Id == manager1Id))
        {
            context.Managers.Add(new Manager
            {
                Id = manager1Id,
                UserId = managerUser.Id,
                FirstName = "Jia Hui",
                LastName = "Lim",
                Email = managerEmail,
                Phone = "+60 11 234 5678",
                Department = "إدارة المرافق والصيانة",
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Manager record.");
        }

        if (manager2User is not null && !await context.Managers.AnyAsync(m => m.Id == manager2Id))
        {
            context.Managers.Add(new Manager
            {
                Id = manager2Id,
                UserId = manager2User.Id,
                FirstName = "Pei Shan",
                LastName = "Low",
                Email = manager2Email,
                Phone = "+60 10 345 6789",
                Department = "إدارة العمليات الميدانية",
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Manager2 record.");
        }

        await context.SaveChangesAsync();

        // ── DataEntries ───────────────────────────────────────────────────────────
        var dataEntry1Id = new Guid("00000000-0000-0000-0041-000000000041");

        if (dataEntryUser is not null && !await context.DataEntries.AnyAsync(d => d.Id == dataEntry1Id))
        {
            context.DataEntries.Add(new DataEntry
            {
                Id = dataEntry1Id,
                UserId = dataEntryUser.Id,
                FirstName = "Mei Lin",
                LastName = "Wong",
                Email = dataEntryEmail,
                Phone = "+60 11 345 6789",
                Section = "العمليات",
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding DataEntry record.");
        }

        var dataEntry2Id = new Guid("00000000-0000-0000-0092-000000000092");

        if (dataEntry2User is not null && !await context.DataEntries.AnyAsync(d => d.Id == dataEntry2Id))
        {
            context.DataEntries.Add(new DataEntry
            {
                Id = dataEntry2Id,
                UserId = dataEntry2User.Id,
                FirstName = "Viknesh",
                LastName = "Menon",
                Email = dataEntry2Email,
                Phone = "+60 16 456 7890",
                Section = "خدمة العملاء",
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding DataEntry2 record.");
        }

        await context.SaveChangesAsync();

        // ── TechnicianGroups ─────────────────────────────────────────────────────
        var group1Id = new Guid("33333333-3333-3333-3333-333333333333");
        var group2Id = new Guid("44444444-4444-4444-4444-444444444444");
        var group3Id = new Guid("00000000-0000-0000-0062-000000000062");
        var group4Id = new Guid("00000000-0000-0000-0093-000000000093");

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group1Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group1Id,
                Name = "Pasukan Kuala Lumpur",
                Description = "فريق متخصص في صيانة المركبات الخفيفة وخدمات الورش في منطقة كوالالمبور",
                LeaderUserId = techUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group2Id,
                Name = "Pasukan Johor Bahru",
                Description = "فريق متخصص في أعمال الصيانة والطوارئ في منطقة جوهور باهرو والمنطقة الجنوبية",
                LeaderUserId = managerUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group3Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group3Id,
                Name = "Pasukan Wilayah Utara",
                Description = "فريق متخصص في صيانة أساطيل النقل والشاحنات الخفيفة في المنطقة الشمالية",
                LeaderUserId = tech3User?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group4Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group4Id,
                Name = "Pasukan Putrajaya",
                Description = "فريق متخصص في صيانة مركبات الضيافة والنقل في بوتراجايا",
                LeaderUserId = manager2User?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Clients ───────────────────────────────────────────────────────────────
        var client1Id = new Guid("00000000-0000-0000-0029-000000000029");
        var client2Id = new Guid("00000000-0000-0000-0030-000000000030");
        var client3Id = new Guid("00000000-0000-0000-0063-000000000063");
        var client4Id = new Guid("00000000-0000-0000-0064-000000000064");
        var client5Id = new Guid("00000000-0000-0000-0065-000000000065");

        if (!await context.Clients.AnyAsync(c => c.Id == client1Id))
        {
            context.Clients.Add(new Client
            {
                Id = client1Id,
                Name = "Viknesh Chan",
                CompanyName = "Pemilik Kenderaan Individu",
                Email = "farid@kualalumpurdev.my",
                Phone = "+60 3 456 7890",
                Address = "Jalan Sultan Ismail، بوكيت بينتانغ، كوالالمبور 50250",
                Notes = "عميل فردي لصيانة دورية وإصلاحات ميكانيكية لمركبته",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client2Id))
        {
            context.Clients.Add(new Client
            {
                Id = client2Id,
                Name = "Daniel Teh",
                CompanyName = "Johor Fleet Transport Sdn. Bhd.",
                Email = "hakim@johorfleet.my",
                Phone = "+60 7 567 8901",
                Address = "Jalan Tun Razak، تامان تون، جوهور باهرو 80250",
                Notes = "عميل أساطيل يحتاج صيانة دورية لـ 45 مركبة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client3Id))
        {
            context.Clients.Add(new Client
            {
                Id = client3Id,
                Name = "Darren Ong",
                CompanyName = "Petronas Logistics Services Sdn. Bhd.",
                Email = "amir@petronasfleet.my",
                Phone = "+60 3 678 9012",
                Address = "Persiaran Sultan، سكشن 13، شاه عالم 40100",
                Notes = "عميل شركات بعقد صيانة لأسطول مركبات ميدانية",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client4Id))
        {
            context.Clients.Add(new Client
            {
                Id = client4Id,
                Name = "Brandon Foong",
                CompanyName = "KL Smart Mobility Sdn. Bhd.",
                Email = "daniel@klrides.my",
                Phone = "+60 7 789 0123",
                Address = "Jalan Ampang، بريسينكت 15، بوتراجايا 62000",
                Notes = "عميل نقل تشاركي يحتاج صيانة سريعة لمركبات الخدمة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client5Id))
        {
            context.Clients.Add(new Client
            {
                Id = client5Id,
                Name = "Kavitha Devi",
                CompanyName = "Putrajaya Medical Centre - Medical Transport Fleet",
                Email = "aina@putrajayamedical.my",
                Phone = "+60 5 890 1234",
                Address = "Persiaran Perdana، غريينتاون، إيبوه 30450",
                Notes = "عميل قطاع صحي لصيانة سيارات الإسعاف والنقل الطبي",
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        // ── Vehicles ─────────────────────────────────────────────────────────────
        var veh1Id = new Guid("AA000001-0000-0000-0000-000000000001");
        var veh2Id = new Guid("AA000002-0000-0000-0000-000000000002");
        var veh3Id = new Guid("AA000003-0000-0000-0000-000000000003");
        var veh4Id = new Guid("AA000004-0000-0000-0000-000000000004");
        var veh5Id = new Guid("AA000005-0000-0000-0000-000000000005");

        if (!await context.Vehicles.AnyAsync(v => v.Id == veh1Id))
        {
            context.Vehicles.Add(new Vehicle
            {
                Id = veh1Id,
                VIN = "1HGBH41JXMN109186",
                Make = "Toyota",
                Model = "Camry",
                Year = 2022,
                LicensePlate = "WKL 1234",
                Color = "أبيض",
                Mileage = 32000,
                EngineType = "2.5L 4-Cylinder",
                TransmissionType = TransmissionType.Automatic,
                FuelType = FuelType.Gasoline,
                OwnerName = "Daniel Ong",
                OwnerPhone = "+60 10 111 2222",
                OwnerEmail = "ahmed.shamri@email.my",
                PurchaseDate = new DateTime(2022, 3, 15),
                LastServiceDate = new DateTime(2024, 9, 10),
                NextServiceDate = new DateTime(2025, 3, 10),
                LastServiceMileage = 28000,
                NextServiceMileage = 38000,
                Status = VehicleStatus.Active,
                Notes = "سيارة سيدان للاستخدام العائلي، صيانة منتظمة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Vehicles.AnyAsync(v => v.Id == veh2Id))
        {
            context.Vehicles.Add(new Vehicle
            {
                Id = veh2Id,
                VIN = "2T1BURHE0JC056984",
                Make = "Toyota",
                Model = "Corolla",
                Year = 2021,
                LicensePlate = "JQH 5678",
                Color = "فضي",
                Mileage = 51000,
                EngineType = "1.8L 4-Cylinder",
                TransmissionType = TransmissionType.Automatic,
                FuelType = FuelType.Gasoline,
                OwnerName = "Mei Ling Lim",
                OwnerPhone = "+60 11 333 4444",
                OwnerEmail = "sara.q@email.my",
                PurchaseDate = new DateTime(2021, 7, 20),
                LastServiceDate = new DateTime(2024, 11, 5),
                NextServiceDate = new DateTime(2025, 5, 5),
                LastServiceMileage = 46000,
                NextServiceMileage = 56000,
                Status = VehicleStatus.Active,
                Notes = "استخدام يومي للتنقل، يحتاج فحص الإطارات في الخدمة القادمة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Vehicles.AnyAsync(v => v.Id == veh3Id))
        {
            context.Vehicles.Add(new Vehicle
            {
                Id = veh3Id,
                VIN = "WBA3A5C51CF256651",
                Make = "BMW",
                Model = "320i",
                Year = 2023,
                LicensePlate = "BSA 9012",
                Color = "أسود",
                Mileage = 15000,
                EngineType = "2.0L TwinPower Turbo",
                TransmissionType = TransmissionType.Automatic,
                FuelType = FuelType.Gasoline,
                OwnerName = "Adrian Nair",
                OwnerPhone = "+60 16 555 6666",
                OwnerEmail = "faisal.otaibi@email.my",
                PurchaseDate = new DateTime(2023, 1, 10),
                LastServiceDate = new DateTime(2024, 8, 20),
                NextServiceDate = new DateTime(2025, 2, 20),
                LastServiceMileage = 10000,
                NextServiceMileage = 20000,
                Status = VehicleStatus.InService,
                Notes = "سيارة فاخرة، تحت الصيانة الدورية في ورشة كوالالمبور",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Vehicles.AnyAsync(v => v.Id == veh4Id))
        {
            context.Vehicles.Add(new Vehicle
            {
                Id = veh4Id,
                VIN = "5XYKT3A19CG225876",
                Make = "Hyundai",
                Model = "Tucson",
                Year = 2020,
                LicensePlate = "PUT 3456",
                Color = "رمادي",
                Mileage = 78000,
                EngineType = "2.0L 4-Cylinder",
                TransmissionType = TransmissionType.Automatic,
                FuelType = FuelType.Diesel,
                OwnerName = "Viknesh Tan",
                OwnerPhone = "+60 19 777 8888",
                OwnerEmail = "m.alghamdi@email.my",
                PurchaseDate = new DateTime(2020, 5, 25),
                LastServiceDate = new DateTime(2024, 7, 15),
                NextServiceDate = new DateTime(2025, 1, 15),
                LastServiceMileage = 70000,
                NextServiceMileage = 80000,
                Status = VehicleStatus.Active,
                Notes = "سيارة دفع رباعي للاستخدام التجاري الخفيف",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Vehicles.AnyAsync(v => v.Id == veh5Id))
        {
            context.Vehicles.Add(new Vehicle
            {
                Id = veh5Id,
                VIN = "JTDKARFU7H3544068",
                Make = "Toyota",
                Model = "Prius",
                Year = 2023,
                LicensePlate = "PNG 7890",
                Color = "أزرق",
                Mileage = 22000,
                EngineType = "1.8L Hybrid",
                TransmissionType = TransmissionType.CVT,
                FuelType = FuelType.Hybrid,
                OwnerName = "Pei Shan Wong",
                OwnerPhone = "+60 14 999 0000",
                OwnerEmail = "noura.dosari@email.my",
                PurchaseDate = new DateTime(2023, 4, 5),
                LastServiceDate = new DateTime(2024, 10, 1),
                NextServiceDate = new DateTime(2025, 4, 1),
                LastServiceMileage = 18000,
                NextServiceMileage = 28000,
                Status = VehicleStatus.Active,
                Notes = "سيارة هجينة موفرة للوقود، خدمة ممتازة ومنتظمة",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Equipment ────────────────────────────────────────────────────────────
        var eq1Id = new Guid("55555555-5555-5555-5555-555555555555");
        var eq2Id = new Guid("66666666-6666-6666-6666-666666666666");
        var eq3Id = new Guid("00000000-0000-0000-0066-000000000066");
        var eq4Id = new Guid("00000000-0000-0000-0094-000000000094");

        if (!await context.Equipments.AnyAsync(e => e.Id == eq1Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq1Id,
                Name = "جهاز فحص أعطال وتشخيص محرك OBD-II",
                SerialNumber = "OBD-RUH-2024-001",
                Model = "Bosch KTS 560",
                Manufacturer = "بوش",
                Location = "ورشة كوالالمبور الرئيسية - قسم التشخيص الإلكتروني",
                InstallationDate = new DateTime(2020, 6, 15),
                LastMaintenanceDate = new DateTime(2024, 1, 10),
                NextMaintenanceDate = new DateTime(2024, 7, 10),
                Status = EquipmentStatus.Operational,
                Notes = "يدعم فحص المحرك وناقل الحركة وأنظمة ABS ووسائد الهواء",
                QrCode = "2020-06-15 جهاز تشخيص OBD-RUH-2024-001",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq2Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq2Id,
                Name = "رافعة هيدروليكية مع محطة تغيير زيت وخدمة فرامل",
                SerialNumber = "LFT-RUH-2024-002",
                Model = "Rotary SPOA10",
                Manufacturer = "روتاري",
                Location = "ورشة كوالالمبور الرئيسية - المسار رقم 2",
                InstallationDate = new DateTime(2019, 3, 22),
                LastMaintenanceDate = new DateTime(2023, 11, 5),
                NextMaintenanceDate = new DateTime(2024, 5, 5),
                Status = EquipmentStatus.UnderMaintenance,
                Notes = "تستخدم لخدمات تغيير الزيت وفحص الفرامل السريع",
                QrCode = "2019-03-22 رافعة هيدروليكية LFT-RUH-2024-002",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq3Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq3Id,
                Name = "ماكينة تبديل الإطارات وترصيص وضبط زوايا",
                SerialNumber = "TWA-JED-2023-003",
                Model = "Hunter Hawkeye Elite",
                Manufacturer = "هانتر",
                Location = "مركز خدمة جوهور باهرو - قسم الإطارات",
                InstallationDate = new DateTime(2021, 9, 10),
                LastMaintenanceDate = new DateTime(2024, 2, 20),
                NextMaintenanceDate = new DateTime(2024, 8, 20),
                Status = EquipmentStatus.Operational,
                Notes = "تخدم تغيير الإطارات والترصيص وضبط زوايا المركبات",
                QrCode = "2021-09-10 ماكينة إطارات TWA-JED-2023-003",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq4Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq4Id,
                Name = "وحدة خدمة مكيف السيارات وفحص البطارية",
                SerialNumber = "ACB-DAM-2022-004",
                Model = "Robinair AC1234-7 + Midtronics DSS-5000",
                Manufacturer = "روبيناير",
                Location = "مركز صيانة شاه عالم - قسم الكهرباء والتكييف",
                InstallationDate = new DateTime(2022, 3, 5),
                LastMaintenanceDate = new DateTime(2024, 1, 15),
                NextMaintenanceDate = new DateTime(2024, 7, 15),
                Status = EquipmentStatus.Operational,
                Notes = "تستخدم لشحن فريون R-1234yf واختبار البطاريات وبادئ التشغيل",
                QrCode = "2022-03-05 وحدة خدمة مكيف ACB-DAM-2022-004",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── HVACSystems ──────────────────────────────────────────────────────────
        var hvac1Id = new Guid("77777777-7777-7777-7777-777777777777");
        var hvac2Id = new Guid("88888888-8888-8888-8888-888888888888");
        var hvac3Id = new Guid("00000000-0000-0000-0067-000000000067");
        var hvac4Id = new Guid("00000000-0000-0000-0095-000000000095");

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac1Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac1Id,
                Name = "منظومة خدمة تكييف سيارات - مركز كوالالمبور",
                SystemType = "خدمة تكييف سيارات",
                Brand = "روبيناير",
                Model = "AC1234-7",
                Capacity = 30m,
                CapacityUnit = "سيارة/يوم",
                RefrigerantType = "R-1234yf",
                Location = "ورشة كوالالمبور الرئيسية",
                InstallationDate = new DateTime(2020, 6, 15),
                LastInspectionDate = new DateTime(2024, 1, 10),
                NextInspectionDate = new DateTime(2024, 7, 10),
                Notes = "منظومة شحن وفحص تسرب تكييف السيارات",
                EquipmentId = eq1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac2Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac2Id,
                Name = "منظومة تشخيص إلكتروني متعدد الأنظمة",
                SystemType = "تشخيص إلكتروني",
                Brand = "بوش",
                Model = "KTS-560",
                Capacity = 40m,
                CapacityUnit = "فحص/يوم",
                RefrigerantType = "غير مطبق",
                Location = "ورشة كوالالمبور - خط الصيانة السريعة",
                InstallationDate = new DateTime(2019, 3, 22),
                LastInspectionDate = new DateTime(2023, 11, 5),
                NextInspectionDate = new DateTime(2024, 5, 5),
                Notes = "تشخيص الأعطال الكهربائية والميكانيكية للمركبات",
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac3Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac3Id,
                Name = "منظومة الإطارات والزوايا - مركز جوهور باهرو",
                SystemType = "خدمة إطارات",
                Brand = "هانتر",
                Model = "Hawkeye-Elite",
                Capacity = 25m,
                CapacityUnit = "سيارة/يوم",
                RefrigerantType = "غير مطبق",
                Location = "مركز خدمة جوهور باهرو - المنطقة الجنوبية",
                InstallationDate = new DateTime(2021, 9, 10),
                LastInspectionDate = new DateTime(2024, 2, 20),
                NextInspectionDate = new DateTime(2024, 8, 20),
                Notes = "منظومة تبديل الإطارات والترصيص وضبط الزوايا",
                EquipmentId = eq3Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac4Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac4Id,
                Name = "منظومة كهرباء المركبات والبطاريات - شاه عالم",
                SystemType = "خدمة كهرباء سيارات",
                Brand = "ميدترونكس",
                Model = "DSS-5000",
                Capacity = 35m,
                CapacityUnit = "فحص/يوم",
                RefrigerantType = "غير مطبق",
                Location = "مركز صيانة شاه عالم - المنطقة الشمالية",
                InstallationDate = new DateTime(2022, 3, 5),
                LastInspectionDate = new DateTime(2024, 1, 15),
                NextInspectionDate = new DateTime(2024, 7, 15),
                Notes = "فحص البطارية والدينمو ونظام التشغيل للمركبات",
                EquipmentId = eq4Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── TaskOrders ───────────────────────────────────────────────────────────
        var task1Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var task2Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var task3Id = new Guid("00000000-0000-0000-0068-000000000068");
        var task4Id = new Guid("00000000-0000-0000-0096-000000000096");
        var task5Id = new Guid("00000000-0000-0000-0097-000000000097");

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task1Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task1Id,
                Title = "خدمة 30 ألف كم وتغيير زيت - مركز كوالالمبور",
                Description = "تنفيذ صيانة دورية 30 ألف كم تشمل تغيير زيت المحرك وفلتر الزيت وفحص الأنظمة الأساسية للمركبة",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                MaintenanceType = MaintenanceType.Inspection,
                ScheduledDate = DateTime.UtcNow.AddDays(-7),
                DueDate = DateTime.UtcNow.AddDays(7),
                Notes = "يشمل فحص زيت القير والفرامل والإطارات",
                CreatedByUserId = adminUserId,
                TechnicianId = tech1Id,
                EquipmentId = eq1Id,
                ArrivalLatitude = 3.1478,
                ArrivalLongitude = 101.7133,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task2Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task2Id,
                Title = "فحص وإصلاح نظام الفرامل - حالة طارئة",
                Description = "تنفيذ فحص عاجل لنظام الفرامل مع استبدال الأقمشة والهوبات حسب نتائج الفحص",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(14),
                DueDate = DateTime.UtcNow.AddDays(30),
                Notes = "يتضمن اختبار أداء ABS ورسوم تشخيص إلكتروني",
                CreatedByUserId = adminUserId,
                GroupId = group1Id,
                EquipmentId = eq2Id,
                ArrivalLatitude = 3.1478,
                ArrivalLongitude = 101.7133,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task3Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task3Id,
                Title = "تشخيص محرك وخدمة 60 ألف كم - مركز جوهور باهرو",
                Description = "تنفيذ صيانة 60 ألف كم تشمل فحص كمبيوتر المحرك وتنظيف البخاخات وخدمة ناقل الحركة",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(-14),
                DueDate = DateTime.UtcNow.AddDays(-7),
                Notes = "تم إنجاز الخدمة مع تحديث سجل الصيانة الإلكتروني",
                CreatedByUserId = adminUserId,
                TechnicianId = tech3Id,
                EquipmentId = eq3Id,
                ArrivalLatitude = 1.4927,
                ArrivalLongitude = 103.7414,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task4Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task4Id,
                Title = "ضبط زوايا واستبدال إطارات - مركز شاه عالم",
                Description = "فحص نظام التعليق وضبط زوايا العجلات واستبدال الإطارات المتآكلة مع الترصيص",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                MaintenanceType = MaintenanceType.Inspection,
                ScheduledDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(5),
                Notes = "يتطلب فحص اتزان العجلات واختبار طريق",
                CreatedByUserId = adminUserId,
                TechnicianId = tech4Id,
                EquipmentId = eq4Id,
                ArrivalLatitude = 3.0738,
                ArrivalLongitude = 101.5183,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task5Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task5Id,
                Title = "إصلاح تكييف سيارات واستبدال بطارية - أسطول كوالالمبور",
                Description = "تنفيذ إصلاحات تكييف سيارات الأسطول مع فحص البطاريات واستبدال التالف قبل موسم الأعياد",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Notes = "أولوية للأسطول التشغيلي خلال موسم الأعياد",
                CreatedByUserId = adminUserId,
                GroupId = group4Id,
                EquipmentId = eq4Id,
                ArrivalLatitude = 2.9264,
                ArrivalLongitude = 101.6964,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Invoices ─────────────────────────────────────────────────────────────
        var inv1Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var inv2Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var inv3Id = new Guid("00000000-0000-0000-0069-000000000069");
        var inv4Id = new Guid("00000000-0000-0000-0098-000000000098");

        if (!await context.Invoices.AnyAsync(i => i.Id == inv1Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv1Id,
                InvoiceNumber = "INV-2024-MY-001",
                ClientName = "Viknesh Chan",
                ClientEmail = "farid@kualalumpurdev.my",
                ClientAddress = "Jalan Sultan Ismail، بوكيت بينتانغ، كوالالمبور 50250",
                IssueDate = DateTime.UtcNow.AddDays(-30),
                DueDate = DateTime.UtcNow,
                SubTotal = 1500m,
                TaxRate = 8m,
                TaxAmount = 120m,
                TotalAmount = 1620m,
                Status = InvoiceStatus.Sent,
                Notes = "فاتورة خدمة 30 ألف كم وتغيير زيت وفلاتر",
                CreatedByUserId = adminUserId,
                TaskOrderId = task1Id,
                ClientId = client1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Invoices.AnyAsync(i => i.Id == inv2Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv2Id,
                InvoiceNumber = "INV-2024-MY-002",
                ClientName = "Johor Fleet Transport Sdn. Bhd.",
                ClientEmail = "accounts@johorfleet.my",
                ClientAddress = "Jalan Tun Razak، تامان تون، جوهور باهرو 80250",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                SubTotal = 3200m,
                TaxRate = 8m,
                TaxAmount = 256m,
                TotalAmount = 3456m,
                Status = InvoiceStatus.Draft,
                Notes = "فاتورة صيانة 60 ألف كم وفحص ناقل الحركة",
                CreatedByUserId = adminUserId,
                TaskOrderId = task3Id,
                ClientId = client2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Invoices.AnyAsync(i => i.Id == inv3Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv3Id,
                InvoiceNumber = "INV-2024-MY-003",
                ClientName = "Petronas Logistics Services Sdn. Bhd.",
                ClientEmail = "billing@petronasfleet.my",
                ClientAddress = "Persiaran Sultan، سكشن 13، شاه عالم 40100",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                SubTotal = 4100m,
                TaxRate = 8m,
                TaxAmount = 328m,
                TotalAmount = 4428m,
                Status = InvoiceStatus.Sent,
                Notes = "فاتورة ضبط زوايا واستبدال إطارات وفحص منظومة التعليق",
                CreatedByUserId = adminUserId,
                TaskOrderId = task4Id,
                ClientId = client3Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        if (!await context.Invoices.AnyAsync(i => i.Id == inv4Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv4Id,
                InvoiceNumber = "INV-2024-MY-004",
                ClientName = "KL Smart Mobility Sdn. Bhd.",
                ClientEmail = "finance@klrides.my",
                ClientAddress = "Jalan Ampang، بريسينكت 15، بوتراجايا 62000",
                IssueDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(25),
                SubTotal = 2400m,
                TaxRate = 8m,
                TaxAmount = 192m,
                TotalAmount = 2592m,
                Status = InvoiceStatus.Draft,
                Notes = "خدمات إصلاح تكييف السيارات واستبدال البطاريات",
                CreatedByUserId = adminUserId,
                TaskOrderId = task5Id,
                ClientId = client4Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        await context.SaveChangesAsync();

        // ── InvoiceLineItems ─────────────────────────────────────────────────────
        var li1Id = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var li2Id = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var li3Id = new Guid("00000000-0000-0000-0001-000000000001");
        var li4Id = new Guid("00000000-0000-0000-0070-000000000070");
        var li5Id = new Guid("00000000-0000-0000-0071-000000000071");
        var li6Id = new Guid("00000000-0000-0000-0099-000000000099");

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li1Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li1Id,
                Description = "أجور خدمة 30 ألف كم (تغيير زيت + فحص شامل)",
                Quantity = 8m,
                UnitPrice = 120m,
                Total = 960m,
                InvoiceId = inv1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li2Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li2Id,
                Description = "مواد استهلاكية (زيت محرك، فلتر زيت، فلتر هواء)",
                Quantity = 3m,
                UnitPrice = 180m,
                Total = 540m,
                InvoiceId = inv1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li3Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li3Id,
                Description = "خدمة 60 ألف كم تشمل فحص ناقل الحركة",
                Quantity = 16m,
                UnitPrice = 200m,
                Total = 3200m,
                InvoiceId = inv2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li4Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li4Id,
                Description = "أعمال ضبط زوايا واستبدال إطارات",
                Quantity = 12m,
                UnitPrice = 200m,
                Total = 2400m,
                InvoiceId = inv3Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li5Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li5Id,
                Description = "قطع غيار نظام الفرامل (أقمشة + هوبات)",
                Quantity = 2m,
                UnitPrice = 850m,
                Total = 1700m,
                InvoiceId = inv3Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.InvoiceLineItems.AnyAsync(l => l.Id == li6Id))
        {
            context.InvoiceLineItems.Add(new InvoiceLineItem
            {
                Id = li6Id,
                Description = "رسوم تشخيص إلكتروني ورسوم التخلص البيئي",
                Quantity = 1m,
                UnitPrice = 2400m,
                Total = 2400m,
                InvoiceId = inv4Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceReports ───────────────────────────────────────────────────
        var report1Id = new Guid("00000000-0000-0000-0002-000000000002");
        var report2Id = new Guid("00000000-0000-0000-0003-000000000003");
        var report3Id = new Guid("00000000-0000-0000-0072-000000000072");
        var report4Id = new Guid("00000000-0000-0000-0100-000000000100");
        var techFullName = techUser is not null ? "Raj Kumar" : "فني غير معروف";

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report1Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report1Id,
                Title = "تقرير صيانة 30 ألف كم - مركز كوالالمبور",
                Content = "تم تنفيذ خدمة 30 ألف كم للمركبة بنجاح. شملت الخدمة تغيير زيت المحرك وفلتر الزيت وفلتر الهواء وفحص نظام الفرامل والإطارات. لم تُرصد أعطال حرجة وتمت إعادة ضبط تذكير الصيانة.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-5),
                LaborHours = 8m,
                MaterialCost = 300m,
                Recommendations = "الالتزام بتغيير الزيت كل 5,000 كم ومراجعة ضغط الإطارات أسبوعياً.",
                TaskOrderId = task1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report2Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report2Id,
                Title = "تقرير تقييم نظام الفرامل",
                Content = "أظهر الفحص المبدئي تآكلًا مرتفعًا في أقمشة الفرامل الأمامية مع تشقق بسيط في الهوبات. تم التوصية بالاستبدال الفوري لتفادي ارتفاع مسافة التوقف.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-2),
                LaborHours = 4m,
                MaterialCost = 150m,
                Recommendations = "استبدال أقمشة وهوبات الفرامل الأمامية وإعادة فحص نظام ABS.",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report3Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report3Id,
                Title = "تقرير صيانة 60 ألف كم - مركز جوهور باهرو",
                Content = "تمت خدمة 60 ألف كم بنجاح، وشملت فحص كمبيوتر المحرك وتنظيف البخاخات وتغيير شمعات الاحتراق وفحص ناقل الحركة. أداء المركبة تحسن بشكل ملحوظ بعد الصيانة.",
                TechnicianName = "Arjun Tan",
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-7),
                LaborHours = 6m,
                MaterialCost = 320m,
                Recommendations = "متابعة فحص ناقل الحركة بعد 10,000 كم ومراقبة استهلاك الوقود.",
                TaskOrderId = task3Id,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report4Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report4Id,
                Title = "تقرير ضبط زوايا وإطارات - مركز شاه عالم",
                Content = "تم تنفيذ ضبط زوايا العجلات الأربع وترصيص الإطارات بعد ملاحظة تآكل غير منتظم. نتائج جهاز القياس ضمن الحدود الموصى بها، وتم تحسين ثبات المركبة أثناء القيادة.",
                TechnicianName = "Kok Beng Lee",
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-1),
                LaborHours = 5m,
                MaterialCost = 200m,
                Recommendations = "تبديل الإطارات الأمامية خلال 8,000 كم وإعادة فحص ميزان الزوايا.",
                TaskOrderId = task4Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        await context.SaveChangesAsync();

        // ── Availability ─────────────────────────────────────────────────────────
        var avail1Id = new Guid("00000000-0000-0000-0004-000000000004");
        var avail2Id = new Guid("00000000-0000-0000-0005-000000000005");
        var avail3Id = new Guid("00000000-0000-0000-0073-000000000073");
        var avail4Id = new Guid("00000000-0000-0000-0101-000000000101");

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
                Notes = "ساعات العمل الاعتيادية",
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
                Notes = "يوم إجازة مجدول",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Availabilities.AnyAsync(a => a.Id == avail3Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech3Id))
        {
            context.Availabilities.Add(new Availability
            {
                Id = avail3Id,
                TechnicianId = tech3Id,
                StartTime = DateTime.UtcNow.Date.AddHours(7),
                EndTime = DateTime.UtcNow.Date.AddHours(16),
                IsAvailable = true,
                Notes = "ساعات العمل الاعتيادية — منطقة جوهور باهرو",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Availabilities.AnyAsync(a => a.Id == avail4Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.Availabilities.Add(new Availability
            {
                Id = avail4Id,
                TechnicianId = tech4Id,
                StartTime = DateTime.UtcNow.Date.AddHours(8),
                EndTime = DateTime.UtcNow.Date.AddHours(17),
                IsAvailable = true,
                Notes = "ساعات العمل الاعتيادية — المنطقة الشمالية",
                CreatedAt = DateTime.UtcNow
            });
        }

        // ── TechnicianGroupMembers ───────────────────────────────────────────────
        var member1Id = new Guid("00000000-0000-0000-0006-000000000006");
        var member2Id = new Guid("00000000-0000-0000-0007-000000000007");
        var member3Id = new Guid("00000000-0000-0000-0074-000000000074");
        var member4Id = new Guid("00000000-0000-0000-0102-000000000102");

        if (!await context.TechnicianGroupMembers.AnyAsync(m => m.Id == member1Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id)
            && await context.TechnicianGroups.AnyAsync(g => g.Id == group1Id))
        {
            context.TechnicianGroupMembers.Add(new TechnicianGroupMember
            {
                Id = member1Id,
                TechnicianId = tech1Id,
                GroupId = group1Id,
                JoinedAt = DateTime.UtcNow.AddMonths(-6),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroupMembers.AnyAsync(m => m.Id == member2Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech2Id)
            && await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroupMembers.Add(new TechnicianGroupMember
            {
                Id = member2Id,
                TechnicianId = tech2Id,
                GroupId = group2Id,
                JoinedAt = DateTime.UtcNow.AddMonths(-4),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroupMembers.AnyAsync(m => m.Id == member3Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech3Id)
            && await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroupMembers.Add(new TechnicianGroupMember
            {
                Id = member3Id,
                TechnicianId = tech3Id,
                GroupId = group2Id,
                JoinedAt = DateTime.UtcNow.AddMonths(-3),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroupMembers.AnyAsync(m => m.Id == member4Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech4Id)
            && await context.TechnicianGroups.AnyAsync(g => g.Id == group3Id))
        {
            context.TechnicianGroupMembers.Add(new TechnicianGroupMember
            {
                Id = member4Id,
                TechnicianId = tech4Id,
                GroupId = group3Id,
                JoinedAt = DateTime.UtcNow.AddMonths(-5),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Documents ────────────────────────────────────────────────────────────
        var doc1Id = new Guid("00000000-0000-0000-0008-000000000008");
        var doc2Id = new Guid("00000000-0000-0000-0009-000000000009");
        var doc3Id = new Guid("00000000-0000-0000-0075-000000000075");
        var doc4Id = new Guid("00000000-0000-0000-0103-000000000103");

        if (!await context.Documents.AnyAsync(d => d.Id == doc1Id))
        {
            context.Documents.Add(new Document
            {
                Id = doc1Id,
                FileName = "vehicle-30k-service-checklist.pdf",
                FileUrl = "/uploads/vehicle-30k-service-checklist.pdf",
                ContentType = "application/pdf",
                FileSize = 204800,
                DocumentType = "قائمة التحقق",
                UploadedByUserId = adminUserId,
                TaskOrderId = task1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Documents.AnyAsync(d => d.Id == doc2Id))
        {
            context.Documents.Add(new Document
            {
                Id = doc2Id,
                FileName = "diagnostic-scanner-manual.pdf",
                FileUrl = "/uploads/diagnostic-scanner-manual.pdf",
                ContentType = "application/pdf",
                FileSize = 1048576,
                DocumentType = "دليل الاستخدام",
                UploadedByUserId = adminUserId,
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Documents.AnyAsync(d => d.Id == doc3Id))
        {
            context.Documents.Add(new Document
            {
                Id = doc3Id,
                FileName = "maintenance-contract-johor-bahru.pdf",
                FileUrl = "/uploads/maintenance-contract-johor-bahru.pdf",
                ContentType = "application/pdf",
                FileSize = 512000,
                DocumentType = "عقد صيانة",
                UploadedByUserId = adminUserId,
                TaskOrderId = task3Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Documents.AnyAsync(d => d.Id == doc4Id))
        {
            context.Documents.Add(new Document
            {
                Id = doc4Id,
                FileName = "wheel-alignment-report-shah-alam.pdf",
                FileUrl = "/uploads/wheel-alignment-report-shah-alam.pdf",
                ContentType = "application/pdf",
                FileSize = 307200,
                DocumentType = "تقرير فحص",
                UploadedByUserId = adminUserId,
                TaskOrderId = task4Id,
                EquipmentId = eq4Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── ChatMessages ─────────────────────────────────────────────────────────
        var msg1Id = new Guid("00000000-0000-0000-0010-000000000010");
        var msg2Id = new Guid("00000000-0000-0000-0011-000000000011");
        var msg3Id = new Guid("00000000-0000-0000-0076-000000000076");
        var msg4Id = new Guid("00000000-0000-0000-0104-000000000104");

        if (techUser is not null && managerUser is not null)
        {
            if (!await context.ChatMessages.AnyAsync(m => m.Id == msg1Id))
            {
                context.ChatMessages.Add(new ChatMessage
                {
                    Id = msg1Id,
                    SenderId = managerUser.Id,
                    SenderName = "Jia Hui Lim",
                    ReceiverId = techUser.Id,
                    Content = "مرحباً، تم تعيين مهمة خدمة 30 ألف كم لك. يرجى البدء بالمركبة الأولى في مركز كوالالمبور.",
                    MessageType = MessageType.Text,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                });
            }

            if (!await context.ChatMessages.AnyAsync(m => m.Id == msg2Id))
            {
                context.ChatMessages.Add(new ChatMessage
                {
                    Id = msg2Id,
                    SenderId = techUser.Id,
                    SenderName = "Raj Kumar",
                    ReceiverId = managerUser.Id,
                    Content = "تم الاستلام، سأبدأ العمل صباح الغد. هل هناك أي تعليمات خاصة لفحص نظام الفرامل؟",
                    MessageType = MessageType.Text,
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                });
            }

            await context.SaveChangesAsync();
        }

        if (tech2User is not null && manager2User is not null)
        {
            if (!await context.ChatMessages.AnyAsync(m => m.Id == msg3Id))
            {
                context.ChatMessages.Add(new ChatMessage
                {
                    Id = msg3Id,
                    SenderId = manager2User.Id,
                    SenderName = "Pei Shan Low",
                    ReceiverId = tech2User.Id,
                    Content = "Arjun، تم تعيين مهمة صيانة 60 ألف كم لمركبات أسطول جوهور باهرو لك. يرجى التنسيق مع مشرف الأسطول قبل البدء.",
                    MessageType = MessageType.Text,
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                });
            }

            if (!await context.ChatMessages.AnyAsync(m => m.Id == msg4Id))
            {
                context.ChatMessages.Add(new ChatMessage
                {
                    Id = msg4Id,
                    SenderId = tech2User.Id,
                    SenderName = "Arjun Tan",
                    ReceiverId = manager2User.Id,
                    Content = "تم التواصل مع مشرف الأسطول. سأبدأ الصيانة يوم السبت القادم في الساعة السابعة صباحاً.",
                    MessageType = MessageType.Text,
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                });
            }

            await context.SaveChangesAsync();
        }

        // ── AppNotifications ─────────────────────────────────────────────────────
        var notif1Id = new Guid("00000000-0000-0000-0012-000000000012");
        var notif2Id = new Guid("00000000-0000-0000-0013-000000000013");
        var notif3Id = new Guid("00000000-0000-0000-0014-000000000014");
        var notif4Id = new Guid("00000000-0000-0000-0077-000000000077");
        var notif5Id = new Guid("00000000-0000-0000-0105-000000000105");

        if (techUser is not null && !await context.AppNotifications.AnyAsync(n => n.Id == notif1Id))
        {
            context.AppNotifications.Add(new AppNotification
            {
                Id = notif1Id,
                UserId = techUser.Id,
                Title = "مهمة جديدة",
                Message = "تم تعيين مهمة خدمة 30 ألف كم لك في مركز كوالالمبور.",
                Type = NotificationType.Info,
                IsRead = false,
                RelatedEntityId = task1Id.ToString(),
                RelatedEntityType = "TaskOrder",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            });
        }

        if (managerUser is not null && !await context.AppNotifications.AnyAsync(n => n.Id == notif2Id))
        {
            context.AppNotifications.Add(new AppNotification
            {
                Id = notif2Id,
                UserId = managerUser.Id,
                Title = "اكتملت المهمة",
                Message = "تم إكمال مهمة خدمة 30 ألف كم بنجاح.",
                Type = NotificationType.Success,
                IsRead = true,
                RelatedEntityId = task1Id.ToString(),
                RelatedEntityType = "TaskOrder",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        if (!await context.AppNotifications.AnyAsync(n => n.Id == notif3Id))
        {
            context.AppNotifications.Add(new AppNotification
            {
                Id = notif3Id,
                UserId = adminUserId,
                Title = "فاتورة متأخرة",
                Message = "الفاتورة INV-2024-MY-001 تقترب من تاريخ استحقاقها.",
                Type = NotificationType.Warning,
                IsRead = false,
                RelatedEntityId = inv1Id.ToString(),
                RelatedEntityType = "Invoice",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        if (tech2User is not null && !await context.AppNotifications.AnyAsync(n => n.Id == notif4Id))
        {
            context.AppNotifications.Add(new AppNotification
            {
                Id = notif4Id,
                UserId = tech2User.Id,
                Title = "تم جدولة مهمة صيانة",
                Message = "تم جدولة مهمة صيانة 60 ألف كم لأسطول جوهور باهرو. الموعد: السبت القادم الساعة 7 صباحاً.",
                Type = NotificationType.Info,
                IsRead = false,
                RelatedEntityId = task3Id.ToString(),
                RelatedEntityType = "TaskOrder",
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            });
        }

        if (!await context.AppNotifications.AnyAsync(n => n.Id == notif5Id))
        {
            context.AppNotifications.Add(new AppNotification
            {
                Id = notif5Id,
                UserId = adminUserId,
                Title = "تنبيه صيانة عاجلة",
                Message = "تم رصد انحراف في نتائج جهاز ضبط الزوايا في مركز شاه عالم. يُوصى بإجراء فحص فوري.",
                Type = NotificationType.Warning,
                IsRead = false,
                RelatedEntityId = eq4Id.ToString(),
                RelatedEntityType = "Equipment",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        await context.SaveChangesAsync();

        // ── EquipmentHealthPredictions ───────────────────────────────────────────
        var pred1Id = new Guid("00000000-0000-0000-0015-000000000015");
        var pred2Id = new Guid("00000000-0000-0000-0016-000000000016");
        var pred3Id = new Guid("00000000-0000-0000-0078-000000000078");
        var pred4Id = new Guid("00000000-0000-0000-0106-000000000106");

        if (!await context.EquipmentHealthPredictions.AnyAsync(p => p.Id == pred1Id))
        {
            context.EquipmentHealthPredictions.Add(new EquipmentHealthPrediction
            {
                Id = pred1Id,
                EquipmentId = eq1Id,
                PredictedFailureDate = DateTime.UtcNow.AddMonths(8),
                FailureProbability = 0.15,
                Recommendation = "جدولة فحص دوري لوصلة جهاز التشخيص وتحديث البرنامج في الخدمة القادمة.",
                TotalInterventions = 5,
                AverageDaysBetweenFailures = 365,
                AverageDaysBetweenMaintenance = 90,
                LastAnalyzedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentHealthPredictions.AnyAsync(p => p.Id == pred2Id))
        {
            context.EquipmentHealthPredictions.Add(new EquipmentHealthPrediction
            {
                Id = pred2Id,
                EquipmentId = eq2Id,
                PredictedFailureDate = DateTime.UtcNow.AddMonths(2),
                FailureProbability = 0.65,
                Recommendation = "فحص منظومة الرفع الهيدروليكي وتغيير زيت النظام قبل التشغيل المكثف.",
                TotalInterventions = 8,
                AverageDaysBetweenFailures = 180,
                AverageDaysBetweenMaintenance = 120,
                LastAnalyzedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentHealthPredictions.AnyAsync(p => p.Id == pred3Id))
        {
            context.EquipmentHealthPredictions.Add(new EquipmentHealthPrediction
            {
                Id = pred3Id,
                EquipmentId = eq3Id,
                PredictedFailureDate = DateTime.UtcNow.AddMonths(12),
                FailureProbability = 0.10,
                Recommendation = "الوحدة في حالة ممتازة. الالتزام بجدول الصيانة الوقائية الحالي كافٍ.",
                TotalInterventions = 3,
                AverageDaysBetweenFailures = 450,
                AverageDaysBetweenMaintenance = 180,
                LastAnalyzedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentHealthPredictions.AnyAsync(p => p.Id == pred4Id))
        {
            context.EquipmentHealthPredictions.Add(new EquipmentHealthPrediction
            {
                Id = pred4Id,
                EquipmentId = eq4Id,
                PredictedFailureDate = DateTime.UtcNow.AddMonths(4),
                FailureProbability = 0.42,
                Recommendation = "إجراء معايرة لجهاز فحص البطاريات وتحديث البرنامج خلال الشهر القادم.",
                TotalInterventions = 6,
                AverageDaysBetweenFailures = 270,
                AverageDaysBetweenMaintenance = 150,
                LastAnalyzedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── EquipmentDigitalTwins ────────────────────────────────────────────────
        var twin1Id = new Guid("00000000-0000-0000-0017-000000000017");
        var twin2Id = new Guid("00000000-0000-0000-0018-000000000018");
        var twin3Id = new Guid("00000000-0000-0000-0079-000000000079");
        var twin4Id = new Guid("00000000-0000-0000-0107-000000000107");

        if (!await context.EquipmentDigitalTwins.AnyAsync(t => t.Id == twin1Id))
        {
            context.EquipmentDigitalTwins.Add(new EquipmentDigitalTwin
            {
                Id = twin1Id,
                EquipmentId = eq1Id,
                CurrentStatus = EquipmentStatus.Operational,
                WearPercentage = 22.5,
                PerformanceScore = 91.0,
                TemperatureCelsius = 18.5,
                UsageHours = 14600,
                LastKnownIssue = "انقطاع متقطع في كابل قراءة وحدة ECU",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "يعمل ضمن المعايير الطبيعية — ورشة كوالالمبور الرئيسية",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentDigitalTwins.AnyAsync(t => t.Id == twin2Id))
        {
            context.EquipmentDigitalTwins.Add(new EquipmentDigitalTwin
            {
                Id = twin2Id,
                EquipmentId = eq2Id,
                CurrentStatus = EquipmentStatus.UnderMaintenance,
                WearPercentage = 61.0,
                PerformanceScore = 63.5,
                TemperatureCelsius = 7.2,
                UsageHours = 21900,
                LastKnownIssue = "انخفاض بسيط في ضغط النظام الهيدروليكي للرافعة",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "مطلوب صيانة وقائية للرافعة قبل ضغط العمل الموسمي",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentDigitalTwins.AnyAsync(t => t.Id == twin3Id))
        {
            context.EquipmentDigitalTwins.Add(new EquipmentDigitalTwin
            {
                Id = twin3Id,
                EquipmentId = eq3Id,
                CurrentStatus = EquipmentStatus.Operational,
                WearPercentage = 15.0,
                PerformanceScore = 95.5,
                TemperatureCelsius = 7.8,
                UsageHours = 10200,
                LastKnownIssue = "لا توجد مشاكل مرصودة",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "أداء ممتاز — مركز خدمة جوهور باهرو",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.EquipmentDigitalTwins.AnyAsync(t => t.Id == twin4Id))
        {
            context.EquipmentDigitalTwins.Add(new EquipmentDigitalTwin
            {
                Id = twin4Id,
                EquipmentId = eq4Id,
                CurrentStatus = EquipmentStatus.Operational,
                WearPercentage = 38.0,
                PerformanceScore = 77.0,
                TemperatureCelsius = 9.5,
                UsageHours = 16800,
                LastKnownIssue = "تفاوت بسيط في قراءات اختبار البطارية لبعض المركبات",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "يعمل بكفاءة مقبولة — مركز صيانة شاه عالم — متابعة مستمرة مطلوبة",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── TechnicianPerformanceScores ──────────────────────────────────────────
        var score1Id = new Guid("00000000-0000-0000-0019-000000000019");
        var score2Id = new Guid("00000000-0000-0000-0020-000000000020");
        var score3Id = new Guid("00000000-0000-0000-0080-000000000080");
        var score4Id = new Guid("00000000-0000-0000-0108-000000000108");

        if (!await context.TechnicianPerformanceScores.AnyAsync(s => s.Id == score1Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.TechnicianPerformanceScores.Add(new TechnicianPerformanceScore
            {
                Id = score1Id,
                TechnicianId = tech1Id,
                AverageInterventionTimeMinutes = 95.0,
                SuccessRate = 0.94,
                CustomerSatisfactionScore = 4.7,
                OnTimeRate = 0.91,
                TotalTasksCompleted = 47,
                TotalTasksDelayed = 4,
                LastCalculatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianPerformanceScores.AnyAsync(s => s.Id == score2Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech2Id))
        {
            context.TechnicianPerformanceScores.Add(new TechnicianPerformanceScore
            {
                Id = score2Id,
                TechnicianId = tech2Id,
                AverageInterventionTimeMinutes = 120.0,
                SuccessRate = 0.88,
                CustomerSatisfactionScore = 4.4,
                OnTimeRate = 0.85,
                TotalTasksCompleted = 32,
                TotalTasksDelayed = 5,
                LastCalculatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianPerformanceScores.AnyAsync(s => s.Id == score3Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech3Id))
        {
            context.TechnicianPerformanceScores.Add(new TechnicianPerformanceScore
            {
                Id = score3Id,
                TechnicianId = tech3Id,
                AverageInterventionTimeMinutes = 105.0,
                SuccessRate = 0.91,
                CustomerSatisfactionScore = 4.6,
                OnTimeRate = 0.89,
                TotalTasksCompleted = 28,
                TotalTasksDelayed = 3,
                LastCalculatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianPerformanceScores.AnyAsync(s => s.Id == score4Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.TechnicianPerformanceScores.Add(new TechnicianPerformanceScore
            {
                Id = score4Id,
                TechnicianId = tech4Id,
                AverageInterventionTimeMinutes = 110.0,
                SuccessRate = 0.87,
                CustomerSatisfactionScore = 4.3,
                OnTimeRate = 0.83,
                TotalTasksCompleted = 21,
                TotalTasksDelayed = 4,
                LastCalculatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── SpareParts ────────────────────────────────────────────────────────────
        var part1Id = new Guid("00000000-0000-0000-0021-000000000021");
        var part2Id = new Guid("00000000-0000-0000-0022-000000000022");
        var part3Id = new Guid("00000000-0000-0000-0023-000000000023");
        var part4Id = new Guid("00000000-0000-0000-0081-000000000081");
        var part5Id = new Guid("00000000-0000-0000-0109-000000000109");

        if (!await context.SpareParts.AnyAsync(p => p.Id == part1Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part1Id,
                Name = "فلتر زيت محرك",
                PartNumber = "OF-5W30-2024",
                Description = "فلتر زيت متوافق مع محركات البنزين رباعية الأسطوانات",
                Unit = "قطعة",
                QuantityInStock = 24,
                MinimumStockLevel = 10,
                UnitPrice = 35.00m,
                Supplier = "شركة قطع غيار السيارات الماليزية",
                StorageLocation = "مستودع كوالالمبور - رف أ3",
                Notes = "يُستبدل مع كل خدمة تغيير زيت",
                QrCode = "فلتر زيت محرك OF-5W30-2024",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part2Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part2Id,
                Name = "طقم أقمشة فرامل أمامية",
                PartNumber = "BP-FRT-2024",
                Description = "أقمشة فرامل أمامية عالية التحمل للسيارات المتوسطة",
                Unit = "قطعة",
                QuantityInStock = 4,
                MinimumStockLevel = 5,
                UnitPrice = 280.00m,
                Supplier = "مؤسسة فرامل آسيا للسيارات",
                StorageLocation = "مستودع كوالالمبور - رف ب7",
                Notes = "مخزون منخفض — أولوية للتوريد",
                QrCode = "أقمشة فرامل BP-FRT-2024",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part3Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part3Id,
                Name = "طقم شمعات احتراق",
                PartNumber = "SPK-IR-SET4",
                Description = "طقم 4 شمعات إيريديوم لمحركات البنزين",
                Unit = "طقم",
                QuantityInStock = 2,
                MinimumStockLevel = 2,
                UnitPrice = 160.00m,
                Supplier = "شركة الإمداد الذكي لقطع السيارات",
                StorageLocation = "مستودع كوالالمبور - رف ج5",
                Notes = "يُستخدم ضمن خدمات 60 ألف كم",
                QrCode = "شمعات احتراق SPK-IR-SET4",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part4Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part4Id,
                Name = "بطارية سيارة 70 أمبير",
                PartNumber = "BAT-70AH-AGM",
                Description = "بطارية AGM بقدرة تشغيل عالية للمركبات التجارية",
                Unit = "قطعة",
                QuantityInStock = 3,
                MinimumStockLevel = 2,
                UnitPrice = 520.00m,
                Supplier = "شركة ماليزيا لبطاريات السيارات",
                StorageLocation = "مستودع شاه عالم - رف ب2",
                Notes = "مناسبة لأسطول النقل الخفيف وسيارات الخدمة",
                QrCode = "بطارية سيارة BAT-70AH-AGM",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part5Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part5Id,
                Name = "سائل تبريد محرك طويل العمر",
                PartNumber = "CLNT-LL-20L",
                Description = "سائل تبريد جاهز للاستخدام لحماية المحرك في الأجواء الحارة",
                Unit = "لتر",
                QuantityInStock = 40,
                MinimumStockLevel = 20,
                UnitPrice = 32.00m,
                Supplier = "شركة ماليزيا لزيوت المحركات",
                StorageLocation = "مستودع جوهور باهرو - رف أ1",
                Notes = "مخصص للمركبات العاملة في مناخ ماليزيا الاستوائي",
                QrCode = "سائل تبريد CLNT-LL-20L",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── SparePartUsages ───────────────────────────────────────────────────────
        var usage1Id = new Guid("00000000-0000-0000-0024-000000000024");
        var usage2Id = new Guid("00000000-0000-0000-0025-000000000025");
        var usage3Id = new Guid("00000000-0000-0000-0082-000000000082");

        if (!await context.SparePartUsages.AnyAsync(u => u.Id == usage1Id)
            && await context.SpareParts.AnyAsync(p => p.Id == part1Id))
        {
            context.SparePartUsages.Add(new SparePartUsage
            {
                Id = usage1Id,
                SparePartId = part1Id,
                TaskOrderId = task1Id,
                QuantityUsed = 3,
                Notes = "تم تغيير فلتر الزيت ضمن خدمة 30 ألف كم",
                UsedAt = DateTime.UtcNow.AddDays(-7),
                UsedByUserId = adminUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SparePartUsages.AnyAsync(u => u.Id == usage2Id)
            && await context.SpareParts.AnyAsync(p => p.Id == part2Id))
        {
            context.SparePartUsages.Add(new SparePartUsage
            {
                Id = usage2Id,
                SparePartId = part2Id,
                TaskOrderId = task2Id,
                QuantityUsed = 2,
                Notes = "تم استبدال أقمشة الفرامل الأمامية ضمن الإصلاح الطارئ",
                UsedAt = DateTime.UtcNow.AddDays(-7),
                UsedByUserId = adminUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SparePartUsages.AnyAsync(u => u.Id == usage3Id)
            && await context.SpareParts.AnyAsync(p => p.Id == part4Id))
        {
            context.SparePartUsages.Add(new SparePartUsage
            {
                Id = usage3Id,
                SparePartId = part4Id,
                TaskOrderId = task5Id,
                QuantityUsed = 1,
                Notes = "تم استبدال بطارية مركبة خدمة ضمن أسطول كوالالمبور",
                UsedAt = DateTime.UtcNow.AddDays(-1),
                UsedByUserId = adminUserId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceSchedules ──────────────────────────────────────────────────
        var sched1Id = new Guid("00000000-0000-0000-0026-000000000026");
        var sched2Id = new Guid("00000000-0000-0000-0027-000000000027");
        var sched3Id = new Guid("00000000-0000-0000-0028-000000000028");
        var sched4Id = new Guid("00000000-0000-0000-0083-000000000083");
        var sched5Id = new Guid("00000000-0000-0000-0110-000000000110");

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched1Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched1Id,
                Name = "برنامج تغيير الزيت الدوري - مركز كوالالمبور",
                Description = "تغيير زيت المحرك وفلتر الزيت وفحص 20 نقطة سلامة بشكل ربع سنوي للمركبات المتعاقدة",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.Quarterly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-7),
                NextDueAt = DateTime.UtcNow.AddDays(83),
                IsActive = true,
                Notes = "استخدام زيت 5W-30 المعتمد فقط",
                CreatedByUserId = adminUserId,
                EquipmentId = eq1Id,
                AssignedTechnicianId = tech1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched2Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched2Id,
                Name = "خطة صيانة 60 ألف كم - أسطول جوهور باهرو",
                Description = "برنامج صيانة شامل يشمل فحص المحرك وناقل الحركة والفرامل والإطارات لأسطول الشركة",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.Annual,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddMonths(-11),
                NextDueAt = DateTime.UtcNow.AddDays(30),
                IsActive = true,
                Notes = "التنفيذ على دفعات وفق جدول المركبات التشغيلي",
                CreatedByUserId = adminUserId,
                EquipmentId = eq2Id,
                AssignedGroupId = group1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched3Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched3Id,
                Name = "فحص موسمي للمركبات قبل الصيف",
                Description = "فحص تكييف المركبات والبطارية وسائل التبريد والإطارات قبل موسم الصيف",
                MaintenanceType = MaintenanceType.Inspection,
                Frequency = ScheduleFrequency.Monthly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-35),
                NextDueAt = DateTime.UtcNow.AddDays(-5),
                IsActive = true,
                Notes = "متأخر — أولوية للأسطول العامل في المدن الحارة",
                CreatedByUserId = adminUserId,
                AssignedTechnicianId = tech1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched4Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched4Id,
                Name = "عقد صيانة أسطول نصف سنوي - بوتراجايا",
                Description = "خدمة نصف سنوية لأسطول النقل الذكي تشمل صيانة ميكانيكية وكهربائية وتقارير جاهزية",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.SemiAnnual,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-14),
                NextDueAt = DateTime.UtcNow.AddMonths(6),
                IsActive = true,
                Notes = "يشمل مركبات الخدمة الموسمية في بوتراجايا",
                CreatedByUserId = adminUserId,
                EquipmentId = eq3Id,
                AssignedGroupId = group2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched5Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched5Id,
                Name = "خطة فحص الطوارئ والمساعدة على الطريق - شاه عالم",
                Description = "فحص دوري لمعدات الطوارئ والبطاريات والإطارات للمركبات الميدانية في المنطقة الشمالية",
                MaintenanceType = MaintenanceType.Inspection,
                Frequency = ScheduleFrequency.Quarterly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-1),
                NextDueAt = DateTime.UtcNow.AddDays(89),
                IsActive = true,
                Notes = "تركيز على جاهزية البطاريات وإطارات المركبات للخدمات العاجلة",
                CreatedByUserId = adminUserId,
                EquipmentId = eq4Id,
                AssignedTechnicianId = tech4Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── PremiumServices ───────────────────────────────────────────────────────
        var premService1Id = new Guid("00000000-0000-0000-0033-000000000033");
        var premService2Id = new Guid("00000000-0000-0000-0034-000000000034");
        var premService3Id = new Guid("00000000-0000-0000-0084-000000000084");

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService1Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService1Id,
                Name = "مساعدة طريق طارئة للمركبات — 24/7",
                Description = "استجابة طارئة على مدار الساعة لأعطال المركبات مثل توقف البطارية أو أعطال التشغيل في جميع مناطق المملكة",
                ServiceType = PremiumServiceType.Emergency,
                Price = 499.00m,
                DurationHours = 4,
                PriorityLevel = TaskPriority.Critical,
                IsActive = true,
                Features = "وصول سريع، فحص بطارية وتشغيل فوري، قطر عند الحاجة، تقارير حادثة إلكترونية",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService2Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService2Id,
                Name = "عقد صيانة أسطول سنوي شامل",
                Description = "صيانة وإصلاح شامل سنوي لمركبات الأسطول مع تقارير أداء دورية وخطة قطع غيار استباقية",
                ServiceType = PremiumServiceType.FullOverhaul,
                Price = 1200.00m,
                DurationHours = 16,
                PriorityLevel = TaskPriority.High,
                IsActive = true,
                Features = "صيانة دورية حسب الكيلومترات، إدارة أعطال، تقارير شهرية، أسعار تفضيلية للقطع",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService3Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService3Id,
                Name = "فحص مركبات قبل الشراء",
                Description = "خدمة فحص احترافية قبل شراء المركبة تشمل المحرك والهيكل والإلكترونيات وإصدار تقرير معتمد",
                ServiceType = PremiumServiceType.Inspection,
                Price = 350.00m,
                DurationHours = 2,
                PriorityLevel = TaskPriority.Medium,
                IsActive = true,
                Features = "فحص شامل 120 نقطة، قراءة كمبيوتر، تقييم حالة الهيكل، تقرير موثق",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── PremiumMaintenanceRequests ────────────────────────────────────────────
        var premReq1Id = new Guid("00000000-0000-0000-0035-000000000035");
        var premReq2Id = new Guid("00000000-0000-0000-0036-000000000036");
        var premReq3Id = new Guid("00000000-0000-0000-0085-000000000085");

        if (!await context.PremiumMaintenanceRequests.AnyAsync(r => r.Id == premReq1Id)
            && await context.Clients.AnyAsync(c => c.Id == client1Id)
            && await context.PremiumServices.AnyAsync(s => s.Id == premService1Id))
        {
            context.PremiumMaintenanceRequests.Add(new PremiumMaintenanceRequest
            {
                Id = premReq1Id,
                ClientId = client1Id,
                PremiumServiceId = premService1Id,
                Status = PremiumMaintenanceStatus.Completed,
                RequestDate = DateTime.UtcNow.AddDays(-20),
                ScheduledDate = DateTime.UtcNow.AddDays(-18),
                Notes = "تنفيذ مساعدة طريق لمركبة متوقفة بسبب عطل بطارية في كوالالمبور",
                Address = "Jalan Sultan Ismail، بوكيت بينتانغ، كوالالمبور 50250",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            });
        }

        if (!await context.PremiumMaintenanceRequests.AnyAsync(r => r.Id == premReq2Id)
            && await context.Clients.AnyAsync(c => c.Id == client2Id)
            && await context.PremiumServices.AnyAsync(s => s.Id == premService2Id))
        {
            context.PremiumMaintenanceRequests.Add(new PremiumMaintenanceRequest
            {
                Id = premReq2Id,
                ClientId = client2Id,
                PremiumServiceId = premService2Id,
                Status = PremiumMaintenanceStatus.PaymentPending,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                ScheduledDate = DateTime.UtcNow.AddDays(25),
                Notes = "طلب عقد صيانة سنوي شامل لأسطول النقل التجاري",
                Address = "Jalan Tun Razak، تامان تون، جوهور باهرو 80250",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        if (!await context.PremiumMaintenanceRequests.AnyAsync(r => r.Id == premReq3Id)
            && await context.Clients.AnyAsync(c => c.Id == client3Id)
            && await context.PremiumServices.AnyAsync(s => s.Id == premService3Id))
        {
            context.PremiumMaintenanceRequests.Add(new PremiumMaintenanceRequest
            {
                Id = premReq3Id,
                ClientId = client3Id,
                PremiumServiceId = premService3Id,
                Status = PremiumMaintenanceStatus.Paid,
                RequestDate = DateTime.UtcNow.AddDays(-8),
                ScheduledDate = DateTime.UtcNow.AddDays(5),
                Notes = "خدمة فحص مركبة قبل الشراء لعميل شركة في شاه عالم",
                Address = "Persiaran Sultan، سكشن 13، شاه عالم 40100",
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            });
        }

        await context.SaveChangesAsync();

        // ── Payments ──────────────────────────────────────────────────────────────
        var payment1Id = new Guid("00000000-0000-0000-0037-000000000037");
        var payment2Id = new Guid("00000000-0000-0000-0086-000000000086");

        if (!await context.Payments.AnyAsync(p => p.Id == payment1Id)
            && await context.PremiumMaintenanceRequests.AnyAsync(r => r.Id == premReq1Id))
        {
            context.Payments.Add(new Payment
            {
                Id = payment1Id,
                PremiumMaintenanceRequestId = premReq1Id,
                Amount = 499.00m,
                Status = PaymentStatus.Completed,
                PaymentMethod = PaymentMethod.CreditCard,
                TransactionId = "TXN-MY-KUL-2024-001",
                PaymentDate = DateTime.UtcNow.AddDays(-20),
                Notes = "دفعة خدمة المساعدة الطارئة على الطريق — كوالالمبور",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            });
        }

        if (!await context.Payments.AnyAsync(p => p.Id == payment2Id)
            && await context.PremiumMaintenanceRequests.AnyAsync(r => r.Id == premReq3Id))
        {
            context.Payments.Add(new Payment
            {
                Id = payment2Id,
                PremiumMaintenanceRequestId = premReq3Id,
                Amount = 350.00m,
                Status = PaymentStatus.Completed,
                PaymentMethod = PaymentMethod.BankTransfer,
                TransactionId = "TXN-MY-SAL-2024-002",
                PaymentDate = DateTime.UtcNow.AddDays(-7),
                Notes = "دفعة خدمة فحص مركبة قبل الشراء — شاه عالم",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceRequests ───────────────────────────────────────────────────
        var req1Id = new Guid("00000000-0000-0000-0038-000000000038");
        var req2Id = new Guid("00000000-0000-0000-0039-000000000039");
        var req3Id = new Guid("00000000-0000-0000-0087-000000000087");
        var req4Id = new Guid("00000000-0000-0000-0111-000000000111");

        if (!await context.MaintenanceRequests.AnyAsync(r => r.Id == req1Id)
            && await context.Clients.AnyAsync(c => c.Id == client1Id))
        {
            context.MaintenanceRequests.Add(new MaintenanceRequest
            {
                Id = req1Id,
                Title = "المركبة لا تستجيب عند التشغيل",
                Description = "مركبة العميل في كوالالمبور لا تعمل صباحاً مع ضعف واضح في البطارية وتأخر تشغيل المحرك.",
                EquipmentDescription = "سيارة سيدان - فحص بطارية ونظام تشغيل",
                RequestDate = DateTime.UtcNow.AddDays(-10),
                Status = MaintenanceRequestStatus.Completed,
                Notes = "تم الحل باستبدال البطارية وتنظيف أقطاب التوصيل",
                ClientId = client1Id,
                TaskOrderId = task1Id,
                InvoiceId = inv1Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        if (!await context.MaintenanceRequests.AnyAsync(r => r.Id == req2Id)
            && await context.Clients.AnyAsync(c => c.Id == client2Id))
        {
            context.MaintenanceRequests.Add(new MaintenanceRequest
            {
                Id = req2Id,
                Title = "صوت احتكاك عند الفرملة",
                Description = "تم الإبلاغ عن صوت احتكاك قوي عند الضغط على الفرامل الأمامية مع انخفاض كفاءة التوقف.",
                EquipmentDescription = "مركبة أسطول - نظام فرامل أمامي",
                RequestDate = DateTime.UtcNow.AddDays(-3),
                Status = MaintenanceRequestStatus.InProgress,
                Notes = "تم جدولة استبدال أقمشة وهوبات الفرامل",
                ClientId = client2Id,
                TaskOrderId = task2Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        if (!await context.MaintenanceRequests.AnyAsync(r => r.Id == req3Id)
            && await context.Clients.AnyAsync(c => c.Id == client3Id))
        {
            context.MaintenanceRequests.Add(new MaintenanceRequest
            {
                Id = req3Id,
                Title = "انحراف في مسار المركبة على السرعات العالية",
                Description = "لوحظ انحراف المركبة إلى اليمين مع تآكل غير متساوٍ في الإطارات الأمامية أثناء القيادة.",
                EquipmentDescription = "مركبة خدمة - فحص زوايا وترصيص",
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Status = MaintenanceRequestStatus.InProgress,
                Notes = "تم إرسال الفني Kok Beng Lee لضبط الزوايا فوراً",
                ClientId = client3Id,
                TaskOrderId = task4Id,
                InvoiceId = inv3Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        if (!await context.MaintenanceRequests.AnyAsync(r => r.Id == req4Id)
            && await context.Clients.AnyAsync(c => c.Id == client4Id))
        {
            context.MaintenanceRequests.Add(new MaintenanceRequest
            {
                Id = req4Id,
                Title = "طلب صيانة أسطول مركبات قبل موسم الأعياد",
                Description = "شركة النقل تحتاج صيانة شاملة لمركبات الأسطول قبل موسم الأعياد لضمان الجاهزية التشغيلية الكاملة.",
                EquipmentDescription = "أسطول مركبات KL Smart Mobility Sdn. Bhd. — جميع الفئات",
                RequestDate = DateTime.UtcNow.AddDays(-1),
                Status = MaintenanceRequestStatus.Pending,
                Notes = "أولوية قصوى — موسم الأعياد خلال شهر",
                ClientId = client4Id,
                TaskOrderId = task5Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        await context.SaveChangesAsync();

        // ── TechnicianGpsLogs ─────────────────────────────────────────────────────
        var gps1Id = new Guid("00000000-0000-0000-0042-000000000042");
        var gps2Id = new Guid("00000000-0000-0000-0043-000000000043");
        var gps3Id = new Guid("00000000-0000-0000-0044-000000000044");
        var gps4Id = new Guid("00000000-0000-0000-0088-000000000088");
        var gps5Id = new Guid("00000000-0000-0000-0112-000000000112");

        if (!await context.TechnicianGpsLogs.AnyAsync(g => g.Id == gps1Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.TechnicianGpsLogs.Add(new TechnicianGpsLog
            {
                Id = gps1Id,
                TechnicianId = tech1Id,
                Latitude = 3.1478,
                Longitude = 101.7133,
                RecordedAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGpsLogs.AnyAsync(g => g.Id == gps2Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.TechnicianGpsLogs.Add(new TechnicianGpsLog
            {
                Id = gps2Id,
                TechnicianId = tech1Id,
                Latitude = 3.1502,
                Longitude = 101.7159,
                RecordedAt = DateTime.UtcNow.AddMinutes(-30),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGpsLogs.AnyAsync(g => g.Id == gps3Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech2Id))
        {
            context.TechnicianGpsLogs.Add(new TechnicianGpsLog
            {
                Id = gps3Id,
                TechnicianId = tech2Id,
                Latitude = 3.1390,
                Longitude = 101.6869,
                RecordedAt = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGpsLogs.AnyAsync(g => g.Id == gps4Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech3Id))
        {
            context.TechnicianGpsLogs.Add(new TechnicianGpsLog
            {
                Id = gps4Id,
                TechnicianId = tech3Id,
                Latitude = 1.4927,
                Longitude = 103.7414,
                RecordedAt = DateTime.UtcNow.AddHours(-3),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGpsLogs.AnyAsync(g => g.Id == gps5Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.TechnicianGpsLogs.Add(new TechnicianGpsLog
            {
                Id = gps5Id,
                TechnicianId = tech4Id,
                Latitude = 3.0738,
                Longitude = 101.5183,
                RecordedAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── AuditLogs ─────────────────────────────────────────────────────────────
        var audit1Id = new Guid("00000000-0000-0000-0045-000000000045");
        var audit2Id = new Guid("00000000-0000-0000-0046-000000000046");
        var audit3Id = new Guid("00000000-0000-0000-0089-000000000089");
        var audit4Id = new Guid("00000000-0000-0000-0113-000000000113");

        if (!await context.AuditLogs.AnyAsync(a => a.Id == audit1Id))
        {
            context.AuditLogs.Add(new AuditLog
            {
                Id = audit1Id,
                EntityType = "TaskOrder",
                EntityId = task1Id.ToString(),
                Action = "تم الإنشاء",
                PerformedByUserId = adminUserId,
                PerformedByName = "Wei Ming Tan",
                Details = "تم إنشاء أمر عمل جديد لخدمة 30 ألف كم — مركز كوالالمبور",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            });
        }

        if (!await context.AuditLogs.AnyAsync(a => a.Id == audit2Id))
        {
            context.AuditLogs.Add(new AuditLog
            {
                Id = audit2Id,
                EntityType = "Equipment",
                EntityId = eq1Id.ToString(),
                Action = "تم التحديث",
                PerformedByUserId = adminUserId,
                PerformedByName = "Wei Ming Tan",
                Details = "تم تحديث حالة جهاز التشخيص الإلكتروني بعد الصيانة",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        if (!await context.AuditLogs.AnyAsync(a => a.Id == audit3Id))
        {
            context.AuditLogs.Add(new AuditLog
            {
                Id = audit3Id,
                EntityType = "MaintenanceRequest",
                EntityId = req3Id.ToString(),
                Action = "تم التعيين",
                PerformedByUserId = adminUserId,
                PerformedByName = "Wei Ming Tan",
                Details = "تم تعيين الفني Kok Beng Lee لمعالجة انحراف الزوايا بمركز شاه عالم",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        if (!await context.AuditLogs.AnyAsync(a => a.Id == audit4Id))
        {
            context.AuditLogs.Add(new AuditLog
            {
                Id = audit4Id,
                EntityType = "Invoice",
                EntityId = inv3Id.ToString(),
                Action = "تم الإرسال",
                PerformedByUserId = adminUserId,
                PerformedByName = "Wei Ming Tan",
                Details = "تم إرسال الفاتورة INV-2024-MY-003 إلى Petronas Logistics Services Sdn. Bhd.",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceRequestAssignments ─────────────────────────────────────────
        var assign1Id = new Guid("00000000-0000-0000-0047-000000000047");
        var assign2Id = new Guid("00000000-0000-0000-0048-000000000048");
        var assign3Id = new Guid("00000000-0000-0000-0090-000000000090");
        var assign4Id = new Guid("00000000-0000-0000-0114-000000000114");

        if (!await context.MaintenanceRequestAssignments.AnyAsync(a => a.Id == assign1Id)
            && await context.MaintenanceRequests.AnyAsync(r => r.Id == req1Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.MaintenanceRequestAssignments.Add(new MaintenanceRequestAssignment
            {
                Id = assign1Id,
                MaintenanceRequestId = req1Id,
                TechnicianId = tech1Id,
                AssignedByUserId = adminUserId,
                AssignedAt = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        if (!await context.MaintenanceRequestAssignments.AnyAsync(a => a.Id == assign2Id)
            && await context.MaintenanceRequests.AnyAsync(r => r.Id == req2Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech2Id))
        {
            context.MaintenanceRequestAssignments.Add(new MaintenanceRequestAssignment
            {
                Id = assign2Id,
                MaintenanceRequestId = req2Id,
                TechnicianId = tech2Id,
                AssignedByUserId = adminUserId,
                AssignedAt = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        if (!await context.MaintenanceRequestAssignments.AnyAsync(a => a.Id == assign3Id)
            && await context.MaintenanceRequests.AnyAsync(r => r.Id == req3Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.MaintenanceRequestAssignments.Add(new MaintenanceRequestAssignment
            {
                Id = assign3Id,
                MaintenanceRequestId = req3Id,
                TechnicianId = tech4Id,
                AssignedByUserId = adminUserId,
                AssignedAt = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }

        if (!await context.MaintenanceRequestAssignments.AnyAsync(a => a.Id == assign4Id)
            && await context.MaintenanceRequests.AnyAsync(r => r.Id == req4Id)
            && await context.Technicians.AnyAsync(t => t.Id == tech3Id))
        {
            context.MaintenanceRequestAssignments.Add(new MaintenanceRequestAssignment
            {
                Id = assign4Id,
                MaintenanceRequestId = req4Id,
                TechnicianId = tech3Id,
                AssignedByUserId = adminUserId,
                AssignedAt = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Domain entity seeding complete.");
    }
}
