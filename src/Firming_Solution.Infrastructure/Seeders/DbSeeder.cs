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
