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

        // ── Seed domain entities ─────────────────────────────────────────────────
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var techUser      = await userManager.FindByEmailAsync(techEmail);
        var managerUser   = await userManager.FindByEmailAsync(managerEmail);
        var dataEntryUser = await userManager.FindByEmailAsync(dataEntryEmail);
        var adminUser     = await userManager.FindByEmailAsync(configuration["AdminSettings:Email"] ?? "admin@maintenance.com");
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

        await context.SaveChangesAsync();

        // ── Managers ─────────────────────────────────────────────────────────────
        var manager1Id = new Guid("00000000-0000-0000-0040-000000000040");

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
                Department = "إدارة المرافق",
                CreatedAt = DateTime.UtcNow
            });
            logger.LogInformation("Seeding Manager record.");
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

        await context.SaveChangesAsync();

        // ── TechnicianGroups ─────────────────────────────────────────────────────
        var group1Id = new Guid("33333333-3333-3333-3333-333333333333");
        var group2Id = new Guid("44444444-4444-4444-4444-444444444444");

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group1Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group1Id,
                Name = "فريق تكييف الرياض",
                Description = "فريق متخصص في صيانة أنظمة التكييف في منطقة الرياض",
                LeaderUserId = techUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.TechnicianGroups.AnyAsync(g => g.Id == group2Id))
        {
            context.TechnicianGroups.Add(new TechnicianGroup
            {
                Id = group2Id,
                Name = "فريق الصيانة العامة",
                Description = "فريق متخصص في أعمال الصيانة العامة والطوارئ",
                LeaderUserId = managerUser?.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── Clients ───────────────────────────────────────────────────────────────
        var client1Id = new Guid("00000000-0000-0000-0029-000000000029");
        var client2Id = new Guid("00000000-0000-0000-0030-000000000030");

        if (!await context.Clients.AnyAsync(c => c.Id == client1Id))
        {
            context.Clients.Add(new Client
            {
                Id = client1Id,
                Name = "محمد العمري",
                CompanyName = "شركة الرياض للتطوير",
                Email = "mohammed@riyadhdev.sa",
                Phone = "+966 11 456 7890",
                Address = "طريق الملك فهد، حي العليا، الرياض 12211",
                Notes = "عميل رئيسي لخدمات التكييف",
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

        await context.SaveChangesAsync();

        // ── Equipment ────────────────────────────────────────────────────────────
        var eq1Id = new Guid("55555555-5555-5555-5555-555555555555");
        var eq2Id = new Guid("66666666-6666-6666-6666-666666666666");

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

        await context.SaveChangesAsync();

        // ── HVACSystems ──────────────────────────────────────────────────────────
        var hvac1Id = new Guid("77777777-7777-7777-7777-777777777777");
        var hvac2Id = new Guid("88888888-8888-8888-8888-888888888888");

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

        await context.SaveChangesAsync();

        // ── TaskOrders ───────────────────────────────────────────────────────────
        var task1Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var task2Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

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

        await context.SaveChangesAsync();

        // ── Invoices ─────────────────────────────────────────────────────────────
        var inv1Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var inv2Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");

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

        await context.SaveChangesAsync();

        // ── MaintenanceReports ───────────────────────────────────────────────────
        var report1Id = new Guid("00000000-0000-0000-0002-000000000002");
        var report2Id = new Guid("00000000-0000-0000-0003-000000000003");
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

        // ── TechnicianGroupMembers ───────────────────────────────────────────────
        var member1Id = new Guid("00000000-0000-0000-0006-000000000006");
        var member2Id = new Guid("00000000-0000-0000-0007-000000000007");

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

        await context.SaveChangesAsync();

        // ── Documents ────────────────────────────────────────────────────────────
        var doc1Id = new Guid("00000000-0000-0000-0008-000000000008");
        var doc2Id = new Guid("00000000-0000-0000-0009-000000000009");

        if (!await context.Documents.AnyAsync(d => d.Id == doc1Id))
        {
            context.Documents.Add(new Document
            {
                Id = doc1Id,
                FileName = "hvac-inspection-checklist.pdf",
                FileUrl = "/uploads/hvac-inspection-checklist.pdf",
                ContentType = "application/pdf",
                FileSize = 204800,
                DocumentType = "Checklist",
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
                DocumentType = "Manual",
                UploadedByUserId = adminUserId,
                EquipmentId = eq2Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── ChatMessages ─────────────────────────────────────────────────────────
        var msg1Id = new Guid("00000000-0000-0000-0010-000000000010");
        var msg2Id = new Guid("00000000-0000-0000-0011-000000000011");

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

        // ── AppNotifications ─────────────────────────────────────────────────────
        var notif1Id = new Guid("00000000-0000-0000-0012-000000000012");
        var notif2Id = new Guid("00000000-0000-0000-0013-000000000013");
        var notif3Id = new Guid("00000000-0000-0000-0014-000000000014");

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

        await context.SaveChangesAsync();

        // ── EquipmentHealthPredictions ───────────────────────────────────────────
        var pred1Id = new Guid("00000000-0000-0000-0015-000000000015");
        var pred2Id = new Guid("00000000-0000-0000-0016-000000000016");

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

        await context.SaveChangesAsync();

        // ── EquipmentDigitalTwins ────────────────────────────────────────────────
        var twin1Id = new Guid("00000000-0000-0000-0017-000000000017");
        var twin2Id = new Guid("00000000-0000-0000-0018-000000000018");

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
                SimulationNotes = "يعمل ضمن المعايير الطبيعية",
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
                SimulationNotes = "مطلوب صيانة فورية لمنع العطل",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── TechnicianPerformanceScores ──────────────────────────────────────────
        var score1Id = new Guid("00000000-0000-0000-0019-000000000019");
        var score2Id = new Guid("00000000-0000-0000-0020-000000000020");

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

        await context.SaveChangesAsync();

        // ── SpareParts ────────────────────────────────────────────────────────────
        var part1Id = new Guid("00000000-0000-0000-0021-000000000021");
        var part2Id = new Guid("00000000-0000-0000-0022-000000000022");
        var part3Id = new Guid("00000000-0000-0000-0023-000000000023");

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
                Supplier = "مؤسسة قطع الغيار الوطنية",
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

        await context.SaveChangesAsync();

        // ── SparePartUsages ───────────────────────────────────────────────────────
        var usage1Id = new Guid("00000000-0000-0000-0024-000000000024");
        var usage2Id = new Guid("00000000-0000-0000-0025-000000000025");

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

        await context.SaveChangesAsync();

        // ── MaintenanceSchedules ──────────────────────────────────────────────────
        var sched1Id = new Guid("00000000-0000-0000-0026-000000000026");
        var sched2Id = new Guid("00000000-0000-0000-0027-000000000027");
        var sched3Id = new Guid("00000000-0000-0000-0028-000000000028");

        if (!await context.MaintenanceSchedules.AnyAsync(s => s.Id == sched1Id))
        {
            context.MaintenanceSchedules.Add(new MaintenanceSchedule
            {
                Id = sched1Id,
                Name = "صيانة وقائية شهرية - المبنى أ",
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
                Name = "صيانة ربع سنوية - المبنى ب",
                Description = "صيانة وقائية شاملة تشمل تغيير الزيت وفحص الختم وتحليل الاهتزاز لوحدة التبريد",
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
                Name = "فحص شهري لأنظمة التكييف",
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

        await context.SaveChangesAsync();

        // ── PremiumServices ───────────────────────────────────────────────────────
        var premService1Id = new Guid("00000000-0000-0000-0033-000000000033");
        var premService2Id = new Guid("00000000-0000-0000-0034-000000000034");

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService1Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService1Id,
                Name = "استجابة طوارئ التكييف",
                Description = "استجابة طارئة على مدار الساعة لحالات أعطال أنظمة التكييف الحرجة",
                ServiceType = PremiumServiceType.Emergency,
                Price = 499.00m,
                DurationHours = 4,
                PriorityLevel = TaskPriority.Critical,
                IsActive = true,
                Features = "استجابة في نفس اليوم، إرسال فني متميز بالأولوية، قطع الغيار مشمولة",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.PremiumServices.AnyAsync(s => s.Id == premService2Id))
        {
            context.PremiumServices.Add(new PremiumService
            {
                Id = premService2Id,
                Name = "صيانة شاملة سنوية لجميع الأنظمة",
                Description = "صيانة وإصلاح شامل سنوي لجميع أنظمة التكييف",
                ServiceType = PremiumServiceType.FullOverhaul,
                Price = 1200.00m,
                DurationHours = 16,
                PriorityLevel = TaskPriority.High,
                IsActive = true,
                Features = "فحص شامل، استبدال القطع، تحسين الأداء",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── PremiumMaintenanceRequests ────────────────────────────────────────────
        var premReq1Id = new Guid("00000000-0000-0000-0035-000000000035");
        var premReq2Id = new Guid("00000000-0000-0000-0036-000000000036");

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

        await context.SaveChangesAsync();

        // ── Payments ──────────────────────────────────────────────────────────────
        var payment1Id = new Guid("00000000-0000-0000-0037-000000000037");

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
                TransactionId = "TXN-2024-SA-001-RDC",
                PaymentDate = DateTime.UtcNow.AddDays(-20),
                Notes = "دفعة خدمة استجابة طوارئ التكييف",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceRequests ───────────────────────────────────────────────────
        var req1Id = new Guid("00000000-0000-0000-0038-000000000038");
        var req2Id = new Guid("00000000-0000-0000-0039-000000000039");

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

        await context.SaveChangesAsync();

        // ── TechnicianGpsLogs ─────────────────────────────────────────────────────
        var gps1Id = new Guid("00000000-0000-0000-0042-000000000042");
        var gps2Id = new Guid("00000000-0000-0000-0043-000000000043");
        var gps3Id = new Guid("00000000-0000-0000-0044-000000000044");

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
                Latitude = 21.5433,
                Longitude = 39.1728,
                RecordedAt = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        // ── AuditLogs ─────────────────────────────────────────────────────────────
        var audit1Id = new Guid("00000000-0000-0000-0045-000000000045");
        var audit2Id = new Guid("00000000-0000-0000-0046-000000000046");

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
                Details = "تم إنشاء أمر عمل جديد لفحص التكييف",
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
                Details = "تم تحديث حالة المعدات",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });
        }

        await context.SaveChangesAsync();

        // ── MaintenanceRequestAssignments ─────────────────────────────────────────
        var assign1Id = new Guid("00000000-0000-0000-0047-000000000047");
        var assign2Id = new Guid("00000000-0000-0000-0048-000000000048");

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

        await context.SaveChangesAsync();
        logger.LogInformation("Domain entity seeding complete.");
    }
}
