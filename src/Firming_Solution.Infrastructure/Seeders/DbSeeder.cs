using Firming_Solution.Domain.Entities;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Firming_Solution.Infrastructure.Seeders;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        await db.Database.MigrateAsync();

        // Seed roles
        string[] roles = ["SuperAdmin", "Manager", "Worker", "Accountant", "Viewer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed Super Admin
        const string adminEmail = "admin@cawfarm.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = "superadmin",
                Email = adminEmail,
                FullName = "Super Administrator",
                EmailConfirmed = true,
                Role = UserRole.SuperAdmin,
                IsActive = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@1234!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }

        // Seed Manager
        const string managerEmail = "manager@cawfarm.com";
        if (await userManager.FindByEmailAsync(managerEmail) is null)
        {
            var manager = new AppUser
            {
                UserName = "farmmanager",
                Email = managerEmail,
                FullName = "Farm Manager",
                EmailConfirmed = true,
                Role = UserRole.Manager,
                IsActive = true
            };
            var result = await userManager.CreateAsync(manager, "Manager@1234!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(manager, "Manager");
        }

        // Seed Worker
        const string workerEmail = "worker@cawfarm.com";
        if (await userManager.FindByEmailAsync(workerEmail) is null)
        {
            var worker = new AppUser
            {
                UserName = "fieldworker",
                Email = workerEmail,
                FullName = "Field Worker",
                EmailConfirmed = true,
                Role = UserRole.Worker,
                IsActive = true
            };
            var result = await userManager.CreateAsync(worker, "Worker@1234!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(worker, "Worker");
        }

        // Seed default Cost Categories
        if (!await db.CostCategoryConfigs.AnyAsync())
        {
            var categories = new[]
            {
                new { Key = "Feed",           Display = "খাবার",           Order = 1 },
                new { Key = "Medicine",       Display = "ওষুধ",            Order = 2 },
                new { Key = "Fertilizer",     Display = "সার / কীটনাশক",  Order = 3 },
                new { Key = "Labour",         Display = "শ্রমিক",          Order = 4 },
                new { Key = "Utility",        Display = "ইউটিলিটি",       Order = 5 },
                new { Key = "Transport",      Display = "পরিবহন",          Order = 6 },
                new { Key = "Breeding",       Display = "প্রজনন",          Order = 7 },
                new { Key = "Infrastructure", Display = "অবকাঠামো",       Order = 8 },
                new { Key = "Other",          Display = "অন্যান্য",       Order = 9 },
            };

            var parentMap = new Dictionary<string, CostCategoryConfig>();
            foreach (var c in categories)
            {
                var cfg = new CostCategoryConfig { CategoryKey = c.Key, DisplayName = c.Display, SortOrder = c.Order };
                db.CostCategoryConfigs.Add(cfg);
                parentMap[c.Key] = cfg;
            }
            await db.SaveChangesAsync();

            // Sub-categories for Medicine
            var medParent = parentMap["Medicine"];
            var medSubs = new[]
            {
                new { Key = "ভ্যাকসিন",          Display = "ভ্যাকসিন (Vaccine)",       Order = 1 },
                new { Key = "অ্যান্টিবায়োটিক",    Display = "অ্যান্টিবায়োটিক",         Order = 2 },
                new { Key = "ভিটামিন ও খনিজ",    Display = "ভিটামিন ও খনিজ",           Order = 3 },
                new { Key = "কৃমিনাশক",           Display = "কৃমিনাশক (Dewormer)",      Order = 4 },
                new { Key = "ধানের ওষুধ",         Display = "ধানের ওষুধ",               Order = 5 },
                new { Key = "আলুর ওষুধ",          Display = "আলুর ওষুধ",                Order = 6 },
                new { Key = "সবজির ওষুধ",         Display = "সবজির ওষুধ",               Order = 7 },
                new { Key = "ফলের ওষুধ",          Display = "ফলের ওষুধ",                Order = 8 },
                new { Key = "পাটের ওষুধ",         Display = "পাটের ওষুধ",               Order = 9 },
                new { Key = "অন্যান্য ওষুধ",      Display = "অন্যান্য ওষুধ",            Order = 10 },
            };
            foreach (var s in medSubs)
                db.CostCategoryConfigs.Add(new CostCategoryConfig { CategoryKey = s.Key, DisplayName = s.Display, ParentId = medParent.Id, SortOrder = s.Order });

            // Sub-categories for Fertilizer
            var fertParent = parentMap["Fertilizer"];
            var fertSubs = new[]
            {
                new { Key = "ইউরিয়া",             Display = "ইউরিয়া (Urea)",           Order = 1 },
                new { Key = "টিএসপি",             Display = "টিএসপি (TSP)",             Order = 2 },
                new { Key = "ডিএপি",              Display = "ডিএপি (DAP)",              Order = 3 },
                new { Key = "এমওপি",              Display = "এমওপি / পটাশ (MOP)",       Order = 4 },
                new { Key = "জিপসাম",             Display = "জিপসাম (Gypsum)",          Order = 5 },
                new { Key = "জিংক সালফেট",        Display = "জিংক সালফেট",             Order = 6 },
                new { Key = "বোরন",               Display = "বোরন (Boron)",             Order = 7 },
                new { Key = "চুন",                Display = "চুন (Lime)",               Order = 8 },
                new { Key = "জৈব সার",            Display = "জৈব সার (Organic)",        Order = 9 },
                new { Key = "কম্পোস্ট",           Display = "কম্পোস্ট (Compost)",       Order = 10 },
                new { Key = "কীটনাশক ধান",        Display = "কীটনাশক — ধান",           Order = 11 },
                new { Key = "কীটনাশক আলু",        Display = "কীটনাশক — আলু",           Order = 12 },
                new { Key = "কীটনাশক সবজি",       Display = "কীটনাশক — সবজি",          Order = 13 },
                new { Key = "আগাছানাশক",           Display = "আগাছানাশক (Herbicide)",   Order = 14 },
                new { Key = "ছত্রাকনাশক",         Display = "ছত্রাকনাশক (Fungicide)",  Order = 15 },
            };
            foreach (var s in fertSubs)
                db.CostCategoryConfigs.Add(new CostCategoryConfig { CategoryKey = s.Key, DisplayName = s.Display, ParentId = fertParent.Id, SortOrder = s.Order });

            await db.SaveChangesAsync();
            logger.LogInformation("Default cost categories seeded.");
        }

        // Seed demo Feed Types
        if (!await db.FeedTypes.AnyAsync())
        {
            db.FeedTypes.AddRange(
                new FeedType { FeedName = "Layer Mash", Category = FeedCategory.Grower, Unit = "kg", CurrentPrice = 45.00m, LastUpdated = DateTime.UtcNow },
                new FeedType { FeedName = "Broiler Starter", Category = FeedCategory.Starter, Unit = "kg", CurrentPrice = 52.00m, LastUpdated = DateTime.UtcNow },
                new FeedType { FeedName = "Rice Bran", Category = FeedCategory.Supplement, Unit = "kg", CurrentPrice = 28.00m, LastUpdated = DateTime.UtcNow },
                new FeedType { FeedName = "Wheat Bran", Category = FeedCategory.Supplement, Unit = "kg", CurrentPrice = 32.00m, LastUpdated = DateTime.UtcNow },
                new FeedType { FeedName = "Fish Meal", Category = FeedCategory.Finisher, Unit = "kg", CurrentPrice = 80.00m, LastUpdated = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
            logger.LogInformation("Feed types seeded.");
        }

        // Seed demo Farm (linked to admin)
        if (!await db.Farms.AnyAsync())
        {
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is not null)
            {
                var farm = new Farm
                {
                    OwnerId = admin.Id,
                    FarmName = "CAW Demo Farm",
                    FarmType = FarmType.Mixed,
                    Location = "Dhaka, Bangladesh",
                    Latitude = 23.8103m,
                    Longitude = 90.4125m,
                    TotalArea = 10.5m,
                    IsActive = true
                };
                db.Farms.Add(farm);
                await db.SaveChangesAsync();

                // Link manager to farm
                var mgr = await userManager.FindByEmailAsync(managerEmail);
                if (mgr is not null)
                {
                    db.UserFarms.Add(new UserFarm { UserId = mgr.Id, FarmId = farm.Id });
                    await db.SaveChangesAsync();
                }

                // Seed demo batch
                var batch = new Batch
                {
                    FarmId = farm.Id,
                    BatchName = "Broiler-Apr-2026",
                    Species = BatchSpecies.Poultry,
                    Breed = "Ross 308",
                    StartDate = new DateTime(2026, 4, 1),
                    PlannedEndDate = new DateTime(2026, 5, 15),
                    InitialCount = 500,
                    InitialWeight_kg = 250.0m,
                    PurchaseCost = 75000.00m,
                    Status = BatchStatus.Active
                };
                db.Batches.Add(batch);
                await db.SaveChangesAsync();
                logger.LogInformation("Demo farm and batch seeded.");
            }
        }
    }
}
