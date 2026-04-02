using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Firming_Solution.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class LandController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    private async Task PopulateFarmsAsync(int? selectedFarmId = null)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(
            await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(),
            "Id", "FarmName", selectedFarmId);
    }

    public async Task<IActionResult> Index()
    {
        var farmIds = await GetFarmIdsAsync();
        var parcels = await db.LandParcels
            .Where(lp => farmIds.Contains(lp.FarmId))
            .Include(lp => lp.Farm)
            .OrderBy(lp => lp.FarmId)
            .ToListAsync();
        return View(parcels);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create()
    {
        await PopulateFarmsAsync();
        return View(new LandParcelViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(LandParcelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateFarmsAsync(model.FarmId);
            return View(model);
        }

        var parcel = new LandParcel
        {
            FarmId        = model.FarmId,
            LandName      = model.LandName,
            Area_Decimal  = model.ToShotangsho(),
            OwnershipType = model.OwnershipType,
            LeaseCostPerSeason = model.LeaseCostPerSeason,
            SoilType      = model.SoilType,
            LastTestedDate = model.LastTestedDate
        };

        db.LandParcels.Add(parcel);
        await db.SaveChangesAsync();
        TempData["Success"] = "জমির তথ্য সফলভাবে যোগ করা হয়েছে।";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var farmIds = await GetFarmIdsAsync();
        var parcel = await db.LandParcels
            .Include(lp => lp.Farm)
            .Include(lp => lp.CropSeasons)
            .Include(lp => lp.FertiliserPlans)
            .FirstOrDefaultAsync(lp => lp.Id == id);
        if (parcel is null || !farmIds.Contains(parcel.FarmId)) return NotFound();
        return View(parcel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var parcel = await db.LandParcels.FindAsync(id);
        if (parcel is null) return NotFound();
        parcel.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "জমির তথ্য মুছে ফেলা হয়েছে।";
        return RedirectToAction(nameof(Index));
    }
}
