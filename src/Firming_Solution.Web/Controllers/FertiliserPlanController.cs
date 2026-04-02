using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class FertiliserPlanController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index()
    {
        var farmIds = await GetFarmIdsAsync();
        var plans = await db.FertiliserPlans
            .Where(fp => farmIds.Contains(fp.Land!.FarmId))
            .Include(fp => fp.Land).ThenInclude(lp => lp!.Farm)
            .Include(fp => fp.Season)
            .OrderByDescending(fp => fp.ApplyDate)
            .ToListAsync();
        return View(plans);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        var lands = await db.LandParcels
            .Where(lp => farmIds.Contains(lp.FarmId))
            .Include(lp => lp.Farm)
            .ToListAsync();
        ViewBag.Lands = new SelectList(
            lands.Select(lp => new {
                lp.Id,
                Display = $"{lp.Farm!.FarmName} — {lp.Area_Decimal:N2} শতাংশ" +
                          (string.IsNullOrEmpty(lp.SoilType) ? "" : $" ({lp.SoilType})")
            }),
            "Id", "Display");
        ViewBag.Seasons = new SelectList(
            await db.CropSeasons.Where(cs => farmIds.Contains(cs.Land!.FarmId)).ToListAsync(),
            "Id", "CropName");
        return View(new FertiliserPlan { ApplyDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(FertiliserPlan model)
    {
        ModelState.Remove("Land");
        ModelState.Remove("Season");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            var lands = await db.LandParcels
                .Where(lp => farmIds.Contains(lp.FarmId))
                .Include(lp => lp.Farm)
                .ToListAsync();
            ViewBag.Lands = new SelectList(
                lands.Select(lp => new {
                    lp.Id,
                    Display = $"{lp.Farm!.FarmName} — {lp.Area_Decimal:N2} শতাংশ" +
                              (string.IsNullOrEmpty(lp.SoilType) ? "" : $" ({lp.SoilType})")
                }),
                "Id", "Display");
            ViewBag.Seasons = new SelectList(
                await db.CropSeasons.Where(cs => farmIds.Contains(cs.Land!.FarmId)).ToListAsync(),
                "Id", "CropName");
            return View(model);
        }
        db.FertiliserPlans.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Fertiliser plan created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkApplied(int id)
    {
        var plan = await db.FertiliserPlans.FindAsync(id);
        if (plan is null) return NotFound();
        plan.IsApplied = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Fertiliser application marked as done.";
        return RedirectToAction(nameof(Index));
    }
}
