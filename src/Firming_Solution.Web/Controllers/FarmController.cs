using Firming_Solution.Domain.Entities;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class FarmController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetAccessibleFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index()
    {
        var ids = await GetAccessibleFarmIdsAsync();
        var farms = await db.Farms
            .Where(f => ids.Contains(f.Id))
            .Include(f => f.Owner)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
        return View(farms);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public IActionResult Create() => View(new Farm());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(Farm model)
    {
        ModelState.Remove("Owner");
        ModelState.Remove("OwnerId");
        if (!ModelState.IsValid) return View(model);

        var user = await userManager.GetUserAsync(User);
        model.OwnerId = user!.Id;
        db.Farms.Add(model);
        await db.SaveChangesAsync();

        // Auto-link owner to farm
        db.UserFarms.Add(new UserFarm { UserId = user.Id, FarmId = model.Id });
        await db.SaveChangesAsync();

        TempData["Success"] = "Farm created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var ids = await GetAccessibleFarmIdsAsync();
        if (!ids.Contains(id)) return Forbid();

        var farm = await db.Farms
            .Include(f => f.Owner)
            .Include(f => f.Batches)
            .Include(f => f.LandParcels)
            .Include(f => f.Investments)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (farm is null) return NotFound();
        return View(farm);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var ids = await GetAccessibleFarmIdsAsync();
        if (!ids.Contains(id)) return Forbid();
        var farm = await db.Farms.FindAsync(id);
        if (farm is null) return NotFound();
        return View(farm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Edit(int id, Farm model)
    {
        ModelState.Remove("Owner");
        ModelState.Remove("OwnerId");
        if (!ModelState.IsValid) return View(model);

        var farm = await db.Farms.FindAsync(id);
        if (farm is null) return NotFound();

        farm.FarmName = model.FarmName;
        farm.FarmType = model.FarmType;
        farm.Location = model.Location;
        farm.Latitude = model.Latitude;
        farm.Longitude = model.Longitude;
        farm.TotalArea = model.TotalArea;
        farm.IsActive = model.IsActive;
        await db.SaveChangesAsync();

        TempData["Success"] = "Farm updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var farm = await db.Farms.FindAsync(id);
        if (farm is null) return NotFound();
        farm.IsDeleted = true;
        farm.IsActive = false;
        await db.SaveChangesAsync();
        TempData["Success"] = "Farm deleted.";
        return RedirectToAction(nameof(Index));
    }
}
