using Firming_Solution.Domain.Entities;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class CropSeasonController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var seasons = await db.CropSeasons
            .Where(cs => farmIds.Contains(cs.Land!.FarmId))
            .Include(cs => cs.Land).ThenInclude(lp => lp!.Farm)
            .OrderByDescending(cs => cs.SowDate)
            .ToListAsync();
        return View(seasons);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(int? landId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Lands = new SelectList(
            await db.LandParcels.Where(lp => farmIds.Contains(lp.FarmId)).ToListAsync(),
            "Id", "LandName", landId);
        return View(new CropSeason { SowDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(CropSeason model)
    {
        ModelState.Remove("Land");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Lands = new SelectList(
                await db.LandParcels.Where(lp => farmIds.Contains(lp.FarmId)).ToListAsync(),
                "Id", "LandName");
            return View(model);
        }
        db.CropSeasons.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Crop season created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> UpdateStatus(int id, CropStatus status)
    {
        var cs = await db.CropSeasons.FindAsync(id);
        if (cs is null) return NotFound();
        cs.Status = status;
        if (status == CropStatus.Harvested) cs.ActualHarvestDate = DateTime.Today;
        await db.SaveChangesAsync();
        TempData["Success"] = $"Crop status updated to {status}.";
        return RedirectToAction(nameof(Index));
    }
}
