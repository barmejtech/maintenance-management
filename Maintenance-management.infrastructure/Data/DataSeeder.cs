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
                FirstName = "عبدالله",
                LastName = "الغامدي",
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
                FirstName = "فهد",
                LastName = "القحطاني",
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
                FirstName = "خالد",
                LastName = "الشهري",
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
                FirstName = "سعود",
                LastName = "الدوسري",
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
                FirstName = "ياسر",
                LastName = "العتيبي",
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
        const string tech2Email = "tech2@maintenance.sa";
        const string tech2Password = "Tech1234!";

        if (await userManager.FindByEmailAsync(tech2Email) is null)
        {
            var tech2 = new ApplicationUser
            {
                UserName = tech2Email,
                Email = tech2Email,
                FirstName = "عمر",
                LastName = "الغامدي",
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
        const string tech3Email = "tech3@maintenance.sa";
        const string tech3Password = "Tech1234!";

        if (await userManager.FindByEmailAsync(tech3Email) is null)
        {
            var tech3 = new ApplicationUser
            {
                UserName = tech3Email,
                Email = tech3Email,
                FirstName = "سالم",
                LastName = "المطيري",
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
        const string manager2Email = "manager2@maintenance.sa";
        const string manager2Password = "Manager123!";

        if (await userManager.FindByEmailAsync(manager2Email) is null)
        {
            var manager2 = new ApplicationUser
            {
                UserName = manager2Email,
                Email = manager2Email,
                FirstName = "نورة",
                LastName = "الحربي",
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
        const string dataEntry2Email = "dataentry2@maintenance.sa";
        const string dataEntry2Password = "DataEntry123!";

        if (await userManager.FindByEmailAsync(dataEntry2Email) is null)
        {
            var dataEntry2 = new ApplicationUser
            {
                UserName = dataEntry2Email,
                Email = dataEntry2Email,
                FirstName = "محمد",
                LastName = "السبيعي",
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

        var techUser       = await userManager.FindByEmailAsync(techEmail);
        var managerUser    = await userManager.FindByEmailAsync(managerEmail);
        var dataEntryUser  = await userManager.FindByEmailAsync(dataEntryEmail);
        var tech2User      = await userManager.FindByEmailAsync(tech2Email);
        var tech3User      = await userManager.FindByEmailAsync(tech3Email);
        var manager2User   = await userManager.FindByEmailAsync(manager2Email);
        var dataEntry2User = await userManager.FindByEmailAsync(dataEntry2Email);
        var adminUser      = await userManager.FindByEmailAsync(configuration["AdminSettings:Email"] ?? "admin@maintenance.com");
        if (adminUser is null)
        {
            logger.LogWarning("Admin user not found; skipping domain entity seeding.");
            return;
        }
        var adminUserId = adminUser.Id;

        // ── Technicians ──────────────────────────────────────────────────────────
        var tech1Id = new Guid("11111111-1111-1111-1111-111111111111");
        var tech2Id = new Guid("22222222-2222-2222-2222-222222222222");

        if (techUser is not null && !await context.Technicians.AnyAsync(t => t.Id == tech1Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech1Id,
                UserId = techUser.Id,
                FirstName = "خالد",
                LastName = "الشهري",
                Email = techEmail,
                Phone = "+966 55 123 4567",
                Specialization = "أنظمة التكييف",
                Address = "حي النسيم، الرياض",
                Latitude = 24.7136,
                Longitude = 46.6753,
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
                FirstName = "فهد",
                LastName = "القحطاني",
                Email = managerEmail,
                Phone = "+966 55 234 5678",
                Specialization = "الصيانة العامة",
                Address = "حي العليا، الرياض",
                Latitude = 24.6877,
                Longitude = 46.6854,
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
                FirstName = "عمر",
                LastName = "الغامدي",
                Email = tech2Email,
                Phone = "+966 50 678 9012",
                Specialization = "أنظمة التبريد والتهوية",
                Address = "حي الروضة، جدة 23434",
                Latitude = 21.5433,
                Longitude = 39.1728,
                Status = TechnicianStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech3 (Jeddah).");
        }

        if (tech3User is not null && !await context.Technicians.AnyAsync(t => t.Id == tech4Id))
        {
            context.Technicians.Add(new Technician
            {
                Id = tech4Id,
                UserId = tech3User.Id,
                FirstName = "سالم",
                LastName = "المطيري",
                Email = tech3Email,
                Phone = "+966 53 789 0123",
                Specialization = "الكهرباء وأنظمة الطاقة",
                Address = "حي الفيصلية، الدمام 32234",
                Latitude = 26.4207,
                Longitude = 50.0888,
                Status = TechnicianStatus.Busy,
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Technician tech4 (Dammam).");
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
                FirstName = "فهد",
                LastName = "القحطاني",
                Email = managerEmail,
                Phone = "+966 55 234 5678",
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
                FirstName = "نورة",
                LastName = "الحربي",
                Email = manager2Email,
                Phone = "+966 50 345 6789",
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
                FirstName = "سعود",
                LastName = "الدوسري",
                Email = dataEntryEmail,
                Phone = "+966 55 345 6789",
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
                FirstName = "محمد",
                LastName = "السبيعي",
                Email = dataEntry2Email,
                Phone = "+966 56 456 7890",
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
                Name = "فريق منطقة الرياض",
                Description = "فريق متخصص في صيانة أنظمة التكييف والمرافق في منطقة الرياض",
                LeaderUserId = techUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group2Id,
                Name = "فريق منطقة جدة",
                Description = "فريق متخصص في أعمال الصيانة والطوارئ في منطقة جدة والمنطقة الغربية",
                LeaderUserId = managerUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group3Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group3Id,
                Name = "فريق المنطقة الشرقية",
                Description = "فريق متخصص في صيانة المنشآت الصناعية والتجارية في المنطقة الشرقية",
                LeaderUserId = tech3User?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group4Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group4Id,
                Name = "فريق منطقة مكة المكرمة",
                Description = "فريق متخصص في صيانة المنشآت الفندقية والتجارية في مكة المكرمة",
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
                Name = "محمد العمري",
                CompanyName = "شركة الرياض للتطوير العقاري",
                Email = "mohammed@riyadhdev.sa",
                Phone = "+966 11 456 7890",
                Address = "طريق الملك فهد، حي العليا، الرياض 12211",
                Notes = "عميل رئيسي لخدمات التكييف والمرافق",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client2Id))
        {
            context.Clients.Add(new Client
            {
                Id = client2Id,
                Name = "أحمد الزهراني",
                CompanyName = "مجموعة جدة التجارية",
                Email = "ahmed@jcg.sa",
                Phone = "+966 12 567 8901",
                Address = "شارع التحلية، حي الروضة، جدة 23434",
                Notes = "عميل خدمات الصيانة الوقائية",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client3Id))
        {
            context.Clients.Add(new Client
            {
                Id = client3Id,
                Name = "عبدالرحمن الشمري",
                CompanyName = "شركة أرامكو للخدمات التقنية",
                Email = "abdulrahman@ast.sa",
                Phone = "+966 13 678 9012",
                Address = "شارع الأمير محمد بن فهد، حي الفيصلية، الدمام 32234",
                Notes = "عميل قطاع النفط والطاقة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client4Id))
        {
            context.Clients.Add(new Client
            {
                Id = client4Id,
                Name = "عبدالعزيز المالكي",
                CompanyName = "فنادق مكة الدولية",
                Email = "aziz@makkahhotels.sa",
                Phone = "+966 12 789 0123",
                Address = "شارع إبراهيم الخليل، المسفلة، مكة المكرمة 24231",
                Notes = "عميل قطاع الضيافة والفنادق",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Clients.AnyAsync(c => c.Id == client5Id))
        {
            context.Clients.Add(new Client
            {
                Id = client5Id,
                Name = "هند القرشي",
                CompanyName = "مجمع المدينة الطبي",
                Email = "hind@mmc.sa",
                Phone = "+966 14 890 1234",
                Address = "شارع أبي ذر، حي العزيزية، المدينة المنورة 42313",
                Notes = "عميل القطاع الصحي — صيانة أنظمة التبريد الطبية",
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
                Name = "وحدة مناولة هواء - المبنى أ",
                SerialNumber = "AHU-RUH-2024-001",
                Model = "يورك YAL",
                Manufacturer = "يورك",
                Location = "الدور الأول، المبنى أ، مجمع الرياض التجاري",
                InstallationDate = new DateTime(2020, 6, 15),
                LastMaintenanceDate = new DateTime(2024, 1, 10),
                NextMaintenanceDate = new DateTime(2024, 7, 10),
                Status = EquipmentStatus.Operational,
                Notes = "صيانة دورية كل 3 أشهر",
                QrCode = "2020-06-15 وحدة مناولة هواء AHU-RUH-2024-001",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq2Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq2Id,
                Name = "وحدة تبريد مركزية - المبنى ب",
                SerialNumber = "CH-RUH-2024-002",
                Model = "ترين RTAC",
                Manufacturer = "ترين",
                Location = "سطح المبنى ب، مجمع الرياض التجاري",
                InstallationDate = new DateTime(2019, 3, 22),
                LastMaintenanceDate = new DateTime(2023, 11, 5),
                NextMaintenanceDate = new DateTime(2024, 5, 5),
                Status = EquipmentStatus.UnderMaintenance,
                Notes = "وحدة تبريد 500 طن",
                QrCode = "2019-03-22 وحدة تبريد مركزية CH-RUH-2024-002",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq3Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq3Id,
                Name = "وحدة تكييف مركزية - برج جدة التجاري",
                SerialNumber = "AC-JED-2023-003",
                Model = "كاريير 30XA",
                Manufacturer = "كاريير",
                Location = "الدور الأرضي، برج جدة التجاري، حي البلد، جدة",
                InstallationDate = new DateTime(2021, 9, 10),
                LastMaintenanceDate = new DateTime(2024, 2, 20),
                NextMaintenanceDate = new DateTime(2024, 8, 20),
                Status = EquipmentStatus.Operational,
                Notes = "نظام تبريد مركزي 400 طن — صيانة نصف سنوية",
                QrCode = "2021-09-10 وحدة تكييف AC-JED-2023-003",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Equipments.AnyAsync(e => e.Id == eq4Id))
        {
            context.Equipments.Add(new Equipment
            {
                Id = eq4Id,
                Name = "مضخة مياه التبريد - مجمع الدمام الصناعي",
                SerialNumber = "PMP-DAM-2022-004",
                Model = "غرونفوس NK 65-160",
                Manufacturer = "غرونفوس",
                Location = "غرفة المعدات، مجمع الدمام الصناعي، حي الدانة، الدمام",
                InstallationDate = new DateTime(2022, 3, 5),
                LastMaintenanceDate = new DateTime(2024, 1, 15),
                NextMaintenanceDate = new DateTime(2024, 7, 15),
                Status = EquipmentStatus.Operational,
                Notes = "مضخة مياه تبريد أساسية بقدرة 75 كيلوواط",
                QrCode = "2022-03-05 مضخة مياه PMP-DAM-2022-004",
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
                Name = "نظام تكييف مركزي - المبنى أ",
                SystemType = "مركزي",
                Brand = "يورك",
                Model = "YAL-300",
                Capacity = 300m,
                CapacityUnit = "طن تبريد",
                RefrigerantType = "R-410A",
                Location = "مجمع الرياض التجاري، المبنى أ",
                InstallationDate = new DateTime(2020, 6, 15),
                LastInspectionDate = new DateTime(2024, 1, 10),
                NextInspectionDate = new DateTime(2024, 7, 10),
                Notes = "النظام الرئيسي للتبريد في المبنى أ",
                EquipmentId = eq1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac2Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac2Id,
                Name = "نظام تبريد - المبنى ب",
                SystemType = "وحدة تبريد مركزية",
                Brand = "ترين",
                Model = "RTAC-500",
                Capacity = 500m,
                CapacityUnit = "طن تبريد",
                RefrigerantType = "R-134a",
                Location = "مجمع الرياض التجاري، المبنى ب",
                InstallationDate = new DateTime(2019, 3, 22),
                LastInspectionDate = new DateTime(2023, 11, 5),
                NextInspectionDate = new DateTime(2024, 5, 5),
                Notes = "نظام التبريد للأدوار من 2 إلى 5",
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac3Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac3Id,
                Name = "نظام تكييف مركزي - برج جدة التجاري",
                SystemType = "وحدة تبريد مائية",
                Brand = "كاريير",
                Model = "30XA-400",
                Capacity = 400m,
                CapacityUnit = "طن تبريد",
                RefrigerantType = "R-410A",
                Location = "برج جدة التجاري، حي البلد، جدة",
                InstallationDate = new DateTime(2021, 9, 10),
                LastInspectionDate = new DateTime(2024, 2, 20),
                NextInspectionDate = new DateTime(2024, 8, 20),
                Notes = "نظام تبريد رئيسي للبرج التجاري",
                EquipmentId = eq3Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.HVACSystems.AnyAsync(h => h.Id == hvac4Id))
        {
            context.HVACSystems.Add(new HVACSystem
            {
                Id = hvac4Id,
                Name = "نظام ضخ المياه الباردة - مجمع الدمام الصناعي",
                SystemType = "نظام ضخ المياه الباردة",
                Brand = "غرونفوس",
                Model = "NK-75KW",
                Capacity = 250m,
                CapacityUnit = "طن تبريد",
                RefrigerantType = "مياه التبريد",
                Location = "مجمع الدمام الصناعي، حي الدانة، الدمام",
                InstallationDate = new DateTime(2022, 3, 5),
                LastInspectionDate = new DateTime(2024, 1, 15),
                NextInspectionDate = new DateTime(2024, 7, 15),
                Notes = "نظام توزيع المياه الباردة للمجمع الصناعي",
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
                Title = "فحص دوري لنظام التكييف - الربع الأول",
                Description = "إجراء الفحص الدوري الربع سنوي لجميع وحدات التكييف في المبنى أ بمجمع الرياض التجاري",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                MaintenanceType = MaintenanceType.Inspection,
                ScheduledDate = DateTime.UtcNow.AddDays(-7),
                DueDate = DateTime.UtcNow.AddDays(7),
                Notes = "فحص الفلاتر والأحزمة ومستويات غاز التبريد",
                CreatedByUserId = adminUserId,
                TechnicianId = tech1Id,
                EquipmentId = eq1Id,
                ArrivalLatitude = 24.7136,
                ArrivalLongitude = 46.6753,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task2Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task2Id,
                Title = "إصلاح طارئ لوحدة التبريد",
                Description = "إصلاح طارئ لوحدة التبريد المركزية في المبنى ب بمجمع الرياض التجاري",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(14),
                DueDate = DateTime.UtcNow.AddDays(30),
                Notes = "يشمل تغيير الزيت وتحليل الاهتزاز",
                CreatedByUserId = adminUserId,
                GroupId = group1Id,
                EquipmentId = eq2Id,
                ArrivalLatitude = 24.7136,
                ArrivalLongitude = 46.6753,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task3Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task3Id,
                Title = "صيانة وقائية لنظام التكييف - برج جدة التجاري",
                Description = "إجراء الصيانة الوقائية الشاملة لنظام التبريد المركزي في برج جدة التجاري — فحص الأنابيب والتوصيلات وضبط منظومة التبريد",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(-14),
                DueDate = DateTime.UtcNow.AddDays(-7),
                Notes = "تم إنجاز الصيانة في الموعد المحدد",
                CreatedByUserId = adminUserId,
                TechnicianId = tech3Id,
                EquipmentId = eq3Id,
                ArrivalLatitude = 21.5433,
                ArrivalLongitude = 39.1728,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task4Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task4Id,
                Title = "فحص مضخة مياه التبريد - مجمع الدمام الصناعي",
                Description = "فحص شامل لمضخة مياه التبريد في المجمع الصناعي بالدمام — قياس الضغط والتدفق وفحص المحركات",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                MaintenanceType = MaintenanceType.Inspection,
                ScheduledDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(5),
                Notes = "يتطلب تحليل اهتزاز متخصص",
                CreatedByUserId = adminUserId,
                TechnicianId = tech4Id,
                EquipmentId = eq4Id,
                ArrivalLatitude = 26.4207,
                ArrivalLongitude = 50.0888,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        if (!await context.TaskOrders.AnyAsync(t => t.Id == task5Id))
        {
            context.TaskOrders.Add(new TaskOrder
            {
                Id = task5Id,
                Title = "تنظيف وفحص وحدات التكييف - فنادق مكة الدولية",
                Description = "تنظيف شامل وفحص كامل لجميع وحدات التكييف في فنادق مكة الدولية — التنظيف العميق للمبخرات والمكثفات",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                MaintenanceType = MaintenanceType.Preventive,
                ScheduledDate = DateTime.UtcNow.AddDays(10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Notes = "جدولة التنفيذ بعد انتهاء موسم الحج",
                CreatedByUserId = adminUserId,
                GroupId = group4Id,
                ArrivalLatitude = 21.3891,
                ArrivalLongitude = 39.8579,
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
                InvoiceNumber = "INV-2024-SA-001",
                ClientName = "شركة الرياض للتطوير",
                ClientEmail = "billing@riyadhdev.sa",
                ClientAddress = "طريق الملك فهد، حي العليا، الرياض 12211",
                IssueDate = DateTime.UtcNow.AddDays(-30),
                DueDate = DateTime.UtcNow,
                SubTotal = 1500m,
                TaxRate = 15m,
                TaxAmount = 225m,
                TotalAmount = 1725m,
                Status = InvoiceStatus.Sent,
                Notes = "خدمات فحص أنظمة التكييف",
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
                InvoiceNumber = "INV-2024-SA-002",
                ClientName = "مجموعة جدة التجارية",
                ClientEmail = "accounts@jcg.sa",
                ClientAddress = "شارع التحلية، حي الروضة، جدة 23434",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                SubTotal = 3200m,
                TaxRate = 15m,
                TaxAmount = 480m,
                TotalAmount = 3680m,
                Status = InvoiceStatus.Draft,
                Notes = "صيانة سنوية لوحدة التبريد المركزية",
                CreatedByUserId = adminUserId,
                ClientId = client2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.Invoices.AnyAsync(i => i.Id == inv3Id))
        {
            context.Invoices.Add(new Invoice
            {
                Id = inv3Id,
                InvoiceNumber = "INV-2024-SA-003",
                ClientName = "شركة أرامكو للخدمات التقنية",
                ClientEmail = "billing@ast.sa",
                ClientAddress = "شارع الأمير محمد بن فهد، حي الفيصلية، الدمام 32234",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                SubTotal = 5800m,
                TaxRate = 15m,
                TaxAmount = 870m,
                TotalAmount = 6670m,
                Status = InvoiceStatus.Sent,
                Notes = "فحص وصيانة منظومة ضخ المياه الباردة — مجمع الدمام الصناعي",
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
                InvoiceNumber = "INV-2024-SA-004",
                ClientName = "فنادق مكة الدولية",
                ClientEmail = "finance@makkahhotels.sa",
                ClientAddress = "شارع إبراهيم الخليل، المسفلة، مكة المكرمة 24231",
                IssueDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(25),
                SubTotal = 2400m,
                TaxRate = 15m,
                TaxAmount = 360m,
                TotalAmount = 2760m,
                Status = InvoiceStatus.Draft,
                Notes = "خدمات صيانة أنظمة التكييف للفندق",
                CreatedByUserId = adminUserId,
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
                Description = "عمالة فحص أنظمة التكييف",
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
                Description = "قطع غيار استبدال الفلاتر",
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
                Description = "صيانة وقائية لوحدة التبريد المركزية",
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
                Description = "أعمال الفحص والقياس لمنظومة الضخ",
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
                Description = "استبدال مجموعة أختام المضخة",
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
                Description = "خدمة تنظيف المبخرات والمكثفات",
                Quantity = 8m,
                UnitPrice = 300m,
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
        var techFullName = techUser is not null ? "خالد الشهري" : "فني غير معروف";

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report1Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report1Id,
                Title = "تقرير فحص التكييف - الربع الأول",
                Content = "تم إجراء الفحص الدوري الربع سنوي لأنظمة التكييف في المبنى أ بمجمع الرياض التجاري. تم استبدال جميع الفلاتر. مستويات غاز التبريد ضمن النطاق المقبول. تم ضبط توتر الحزام في وحدة مناولة الهواء. لم يتم اكتشاف أي مشاكل رئيسية.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-5),
                LaborHours = 8m,
                MaterialCost = 300m,
                Recommendations = "جدولة تنظيف عميق سنوي في الربع القادم.",
                TaskOrderId = task1Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report2Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report2Id,
                Title = "تقييم نظام التبريد المركزي",
                Content = "تقييم ما قبل الصيانة لوحدة التبريد المركزية في المبنى ب. تم رصد تسرب طفيف في زيت ختم المضخة. مستويات الاهتزاز مرتفعة قليلاً. الوحدة لا تزال تعمل لكن الصيانة الوقائية ضرورية.",
                TechnicianName = techFullName,
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-2),
                LaborHours = 4m,
                MaterialCost = 50m,
                Recommendations = "استبدال ختم المضخة خلال الصيانة المجدولة. طلب غاز التبريد R-134a.",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report3Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report3Id,
                Title = "تقرير صيانة نظام التكييف - برج جدة التجاري",
                Content = "تمت الصيانة الوقائية الشاملة لنظام التبريد المركزي في برج جدة التجاري. تم تنظيف المبخرات والمكثفات. فحص جميع توصيلات الأنابيب ولم يرصد أي تسرب. تم ضبط ضغط غاز التبريد R-410A على القيم القياسية. الأداء العام ممتاز.",
                TechnicianName = "عمر الغامدي",
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-7),
                LaborHours = 6m,
                MaterialCost = 150m,
                Recommendations = "يُوصى بتغيير سائل التبريد خلال العام القادم. جدولة تنظيف عميق سنوي.",
                TaskOrderId = task3Id,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            });
        }

        if (!await context.MaintenanceReports.AnyAsync(r => r.Id == report4Id))
        {
            context.MaintenanceReports.Add(new MaintenanceReport
            {
                Id = report4Id,
                Title = "تقرير فحص مضخة مياه التبريد - مجمع الدمام الصناعي",
                Content = "تم إجراء فحص شامل لمضخة مياه التبريد في مجمع الدمام الصناعي. قياسات الضغط تعمل ضمن النطاق المقبول (3.2 بار). مستوى الاهتزاز طبيعي. تم اكتشاف تآكل طفيف في مجموعة الأختام يستوجب الاستبدال في الزيارة القادمة.",
                TechnicianName = "سالم المطيري",
                CreatedByUserId = adminUserId,
                ReportDate = DateTime.UtcNow.AddDays(-1),
                LaborHours = 5m,
                MaterialCost = 200m,
                Recommendations = "استبدال مجموعة أختام المضخة فور توفرها. جدولة فحص تفصيلي للمحرك الكهربائي.",
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
                Notes = "ساعات العمل الاعتيادية — منطقة جدة",
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
                Notes = "ساعات العمل الاعتيادية — المنطقة الشرقية",
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
                FileName = "hvac-inspection-checklist.pdf",
                FileUrl = "/uploads/hvac-inspection-checklist.pdf",
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
                FileName = "chiller-unit-manual.pdf",
                FileUrl = "/uploads/chiller-unit-manual.pdf",
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
                FileName = "maintenance-contract-jeddah.pdf",
                FileUrl = "/uploads/maintenance-contract-jeddah.pdf",
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
                FileName = "pump-inspection-report-dammam.pdf",
                FileUrl = "/uploads/pump-inspection-report-dammam.pdf",
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
                    SenderName = "فهد القحطاني",
                    ReceiverId = techUser.Id,
                    Content = "مرحبا، تم تعيين مهمة الفحص الدوري لك. يرجى البدء بالمبنى أ في مجمع الرياض التجاري.",
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
                    SenderName = "خالد الشهري",
                    ReceiverId = managerUser.Id,
                    Content = "تم الاستلام، سأبدأ العمل صباح الغد. هل هناك أي تعليمات خاصة لفحص وحدة التبريد؟",
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
                    SenderName = "نورة الحربي",
                    ReceiverId = tech2User.Id,
                    Content = "عمر، تم تعيين مهمة صيانة برج جدة التجاري لك. يرجى التنسيق مع إدارة المبنى قبل البدء.",
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
                    SenderName = "عمر الغامدي",
                    ReceiverId = manager2User.Id,
                    Content = "تم التواصل مع إدارة المبنى. سأبدأ الصيانة الوقائية يوم السبت القادم في الساعة السابعة صباحاً.",
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
                Message = "تم تعيين مهمة فحص دوري لنظام التكييف لك في مجمع الرياض التجاري.",
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
                Message = "تم إكمال مهمة فحص التكييف - الربع الأول بنجاح.",
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
                Message = "الفاتورة INV-2024-SA-001 تقترب من تاريخ استحقاقها.",
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
                Message = "تم جدولة مهمة صيانة وقائية في برج جدة التجاري. الموعد: السبت القادم الساعة 7 صباحاً.",
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
                Message = "تم رصد ارتفاع في مستوى اهتزاز مضخة مياه التبريد في مجمع الدمام الصناعي. يُوصى بإجراء فحص فوري.",
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
                Recommendation = "جدولة استبدال الفلاتر وفحص الحزام في الخدمة الربع سنوية القادمة.",
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
                Recommendation = "مطلوب استبدال ختم المضخة فوراً. جدولة صيانة طارئة.",
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
                Recommendation = "استبدال مجموعة الأختام خلال الشهر القادم. فحص المحرب الكهربائي وقياس العزل.",
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
                LastKnownIssue = "تآكل طفيف في الحزام لوحظ خلال آخر فحص",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "يعمل ضمن المعايير الطبيعية — مجمع الرياض التجاري",
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
                LastKnownIssue = "تسرب زيت في ختم المضخة؛ مستويات اهتزاز مرتفعة",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "مطلوب صيانة فورية لمنع العطل — مجمع الرياض التجاري",
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
                SimulationNotes = "أداء ممتاز — برج جدة التجاري",
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
                LastKnownIssue = "تآكل في مجموعة الأختام يستوجب الاستبدال قريباً",
                LastSyncedAt = DateTime.UtcNow,
                SimulationNotes = "يعمل بكفاءة مقبولة — مجمع الدمام الصناعي — متابعة مستمرة مطلوبة",
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
                Name = "فلتر هواء MERV-13",
                PartNumber = "AF-MERV13-2024",
                Description = "فلتر هواء عالي الكفاءة لأنظمة التكييف المركزية",
                Unit = "قطعة",
                QuantityInStock = 24,
                MinimumStockLevel = 10,
                UnitPrice = 18.50m,
                Supplier = "شركة التوريدات السعودية للتكييف",
                StorageLocation = "مستودع الرياض - رف أ3",
                Notes = "يُستبدل كل 3 أشهر",
                QrCode = "فلتر هواء MERV-13 AF-MERV13-2024",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part2Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part2Id,
                Name = "حزام مروحة V-Belt B68",
                PartNumber = "VB-B68-2024",
                Description = "حزام مروحة لوحدات مناولة الهواء",
                Unit = "قطعة",
                QuantityInStock = 4,
                MinimumStockLevel = 5,
                UnitPrice = 145.00m,
                Supplier = "مؤسسة قطع الغيار الوطنية للتقنية",
                StorageLocation = "مستودع الرياض - رف ب7",
                Notes = "مخزون منخفض — يُطلب فوراً",
                QrCode = "حزام مروحة V-Belt B68 VB-B68-2024",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part3Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part3Id,
                Name = "طقم ختم مضخة ترين",
                PartNumber = "PSK-RTAC-STD",
                Description = "طقم ختم مضخة قياسي لوحدة التبريد ترين RTAC",
                Unit = "طقم",
                QuantityInStock = 2,
                MinimumStockLevel = 2,
                UnitPrice = 89.00m,
                Supplier = "شركة التوريدات السعودية للتكييف",
                StorageLocation = "مستودع الرياض - رف ج5",
                Notes = "متوافق مع وحدة التبريد المركزية بالمبنى ب",
                QrCode = "طقم ختم مضخة PSK-RTAC-STD",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part4Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part4Id,
                Name = "مجموعة أختام غرونفوس NK",
                PartNumber = "GF-NK-SEAL-75",
                Description = "مجموعة أختام ميكانيكية لمضخة غرونفوس NK 65-160",
                Unit = "طقم",
                QuantityInStock = 3,
                MinimumStockLevel = 2,
                UnitPrice = 320.00m,
                Supplier = "مؤسسة الخليج للمضخات والأنظمة الهيدروليكية",
                StorageLocation = "مستودع الدمام - رف ب2",
                Notes = "مناسب لمضخة التبريد في مجمع الدمام الصناعي",
                QrCode = "أختام غرونفوس GF-NK-SEAL-75",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SpareParts.AnyAsync(p => p.Id == part5Id))
        {
            context.SpareParts.Add(new SparePart
            {
                Id = part5Id,
                Name = "زيت تبريد شيل فريجوليا",
                PartNumber = "SH-FRG-20L",
                Description = "زيت تبريد اصطناعي عالي الأداء للضواغط التجارية",
                Unit = "لتر",
                QuantityInStock = 40,
                MinimumStockLevel = 20,
                UnitPrice = 45.00m,
                Supplier = "شركة شيل للمواد التشحيمية — المملكة العربية السعودية",
                StorageLocation = "مستودع جدة - رف أ1",
                Notes = "متوافق مع غاز التبريد R-134a و R-410A",
                QrCode = "زيت تبريد SH-FRG-20L",
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
                Notes = "تم تغيير الفلاتر في وحدات مناولة الهواء بالمبنى أ",
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
                TaskOrderId = task1Id,
                QuantityUsed = 1,
                Notes = "تم استبدال حزام المروحة في وحدة مناولة الهواء الرئيسية بالمبنى أ",
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
                TaskOrderId = task4Id,
                QuantityUsed = 1,
                Notes = "استبدال مجموعة الأختام في مضخة مياه التبريد بمجمع الدمام الصناعي",
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
                Name = "صيانة وقائية ربع سنوية - مجمع الرياض التجاري",
                Description = "استبدال فلاتر التكييف وفحص توتر الحزام كل ربع سنة في مجمع الرياض التجاري",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.Quarterly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-7),
                NextDueAt = DateTime.UtcNow.AddDays(83),
                IsActive = true,
                Notes = "استخدام فلاتر MERV-13 فقط",
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
                Name = "صيانة سنوية شاملة - وحدة التبريد المركزي",
                Description = "صيانة وقائية شاملة تشمل تغيير الزيت وفحص الختم وتحليل الاهتزاز لوحدة التبريد المركزية",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.Annual,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddMonths(-11),
                NextDueAt = DateTime.UtcNow.AddDays(30),
                IsActive = true,
                Notes = "التنسيق مع فريق خدمة ترين السعودية",
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
                Name = "فحص شهري لأنظمة التكييف - مجمع الرياض",
                Description = "الفحص البصري لجميع مكونات التكييف والتحقق من الأداء في مجمع الرياض التجاري",
                MaintenanceType = MaintenanceType.Inspection,
                Frequency = ScheduleFrequency.Monthly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-35),
                NextDueAt = DateTime.UtcNow.AddDays(-5),
                IsActive = true,
                Notes = "متأخر — يُجدول فوراً",
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
                Name = "صيانة نصف سنوية - برج جدة التجاري",
                Description = "الصيانة الوقائية نصف السنوية لنظام التبريد المركزي في برج جدة التجاري شاملة تنظيف المبخرات والمكثفات",
                MaintenanceType = MaintenanceType.Preventive,
                Frequency = ScheduleFrequency.SemiAnnual,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-14),
                NextDueAt = DateTime.UtcNow.AddMonths(6),
                IsActive = true,
                Notes = "الفريق المختص: فريق منطقة جدة",
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
                Name = "فحص دوري - مضخة مياه التبريد بالدمام",
                Description = "الفحص الدوري لمضخة مياه التبريد في مجمع الدمام الصناعي — قياس الضغط والتدفق ومراقبة الاهتزاز",
                MaintenanceType = MaintenanceType.Inspection,
                Frequency = ScheduleFrequency.Quarterly,
                FrequencyValue = 1,
                LastExecutedAt = DateTime.UtcNow.AddDays(-1),
                NextDueAt = DateTime.UtcNow.AddDays(89),
                IsActive = true,
                Notes = "اهتمام خاص بمستويات الاهتزاز والأختام",
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
                Name = "استجابة طوارئ التكييف — 24/7",
                Description = "استجابة طارئة على مدار الساعة لحالات أعطال أنظمة التكييف الحرجة في جميع مناطق المملكة",
                ServiceType = PremiumServiceType.Emergency,
                Price = 499.00m,
                DurationHours = 4,
                PriorityLevel = TaskPriority.Critical,
                IsActive = true,
                Features = "استجابة في نفس اليوم، إرسال فني متميز بالأولوية، قطع الغيار مشمولة، ضمان سنوي",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService2Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService2Id,
                Name = "صيانة شاملة سنوية — عقد صيانة كامل",
                Description = "صيانة وإصلاح شامل سنوي لجميع أنظمة التكييف والمرافق مع تقارير دورية ومتابعة مستمرة",
                ServiceType = PremiumServiceType.FullOverhaul,
                Price = 1200.00m,
                DurationHours = 16,
                PriorityLevel = TaskPriority.High,
                IsActive = true,
                Features = "فحص شامل، استبدال القطع البالية، تحسين الأداء، تقرير سنوي تفصيلي",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService3Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService3Id,
                Name = "مراقبة أنظمة التكييف عن بُعد",
                Description = "خدمة مراقبة استباقية لأنظمة التكييف عبر أجهزة الاستشعار الذكية مع تنبيهات فورية عند أي خلل",
                ServiceType = PremiumServiceType.Inspection,
                Price = 350.00m,
                DurationHours = 2,
                PriorityLevel = TaskPriority.Medium,
                IsActive = true,
                Features = "مراقبة مستمرة 24/7، تنبيهات فورية، تقارير أسبوعية، تدخل سريع عند الحاجة",
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
                Notes = "إصلاح طارئ لعطل وحدة مناولة الهواء في المبنى أ بمجمع الرياض التجاري",
                Address = "طريق الملك فهد، حي العليا، الرياض 12211",
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
                Notes = "صيانة شاملة سنوية لجميع أنظمة التكييف",
                Address = "شارع التحلية، حي الروضة، جدة 23434",
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
                Notes = "تفعيل خدمة المراقبة عن بُعد لمنظومة ضخ المياه الباردة في مجمع الدمام الصناعي",
                Address = "شارع الأمير محمد بن فهد، حي الفيصلية، الدمام 32234",
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
                TransactionId = "TXN-SA-RUH-2024-001",
                PaymentDate = DateTime.UtcNow.AddDays(-20),
                Notes = "دفعة خدمة استجابة طوارئ التكييف — الرياض",
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
                TransactionId = "TXN-SA-DAM-2024-002",
                PaymentDate = DateTime.UtcNow.AddDays(-7),
                Notes = "دفعة خدمة المراقبة عن بُعد — الدمام",
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
                Title = "وحدة مناولة الهواء لا تبرد",
                Description = "وحدة مناولة الهواء في المبنى أ بمجمع الرياض التجاري لا تنتج هواء بارد. ترتفع درجة الحرارة في مساحات المكاتب.",
                EquipmentDescription = "وحدة مناولة هواء - المبنى أ، الدور الأول، مجمع الرياض التجاري",
                RequestDate = DateTime.UtcNow.AddDays(-10),
                Status = MaintenanceRequestStatus.Completed,
                Notes = "تم الحل خلال الفحص الربع سنوي — تم تعبئة غاز التبريد",
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
                Title = "وحدة التبريد تصدر ضوضاء غير اعتيادية",
                Description = "وحدة التبريد المركزية في المبنى ب بمجمع الرياض التجاري تصدر صوت طحن ومستويات اهتزاز مرتفعة.",
                EquipmentDescription = "وحدة تبريد مركزية ترين RTAC، سطح المبنى ب، مجمع الرياض التجاري",
                RequestDate = DateTime.UtcNow.AddDays(-3),
                Status = MaintenanceRequestStatus.InProgress,
                Notes = "تم جدولة استبدال ختم المضخة",
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
                Title = "انخفاض في ضغط مياه التبريد",
                Description = "رصد انخفاض ملحوظ في ضغط مياه التبريد في مجمع الدمام الصناعي. مستوى الضغط 2.1 بار بدلاً من 3.2 بار المعتاد.",
                EquipmentDescription = "مضخة مياه التبريد - غرفة المعدات، مجمع الدمام الصناعي",
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Status = MaintenanceRequestStatus.InProgress,
                Notes = "تم إرسال الفني سالم المطيري للفحص الفوري",
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
                Title = "طلب صيانة أنظمة التكييف قبل موسم الحج",
                Description = "الفندق يحتاج لصيانة شاملة لجميع وحدات التكييف استعداداً لموسم الحج. المطلوب التأكد من الكفاءة القصوى لجميع الأنظمة.",
                EquipmentDescription = "وحدات تكييف فنادق مكة الدولية — جميع الطوابق",
                RequestDate = DateTime.UtcNow.AddDays(-1),
                Status = MaintenanceRequestStatus.Pending,
                Notes = "أولوية قصوى — موسم الحج خلال شهر",
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
                Latitude = 24.7136,
                Longitude = 46.6753,
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
                Latitude = 24.7200,
                Longitude = 46.6800,
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
                Latitude = 24.6877,
                Longitude = 46.6854,
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
                Latitude = 21.5433,
                Longitude = 39.1728,
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
                Latitude = 26.4207,
                Longitude = 50.0888,
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
                PerformedByName = "عبدالله الغامدي",
                Details = "تم إنشاء أمر عمل جديد لفحص التكييف الربع سنوي — مجمع الرياض التجاري",
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
                PerformedByName = "عبدالله الغامدي",
                Details = "تم تحديث حالة معدات وحدة مناولة الهواء بعد الصيانة",
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
                PerformedByName = "عبدالله الغامدي",
                Details = "تم تعيين الفني سالم المطيري لمعالجة انخفاض ضغط المياه في مجمع الدمام الصناعي",
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
                PerformedByName = "عبدالله الغامدي",
                Details = "تم إرسال الفاتورة INV-2024-SA-003 إلى شركة أرامكو للخدمات التقنية",
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
