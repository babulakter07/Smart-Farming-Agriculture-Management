using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class CostController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index(int? farmId, int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        var query = db.Costs.Where(c => farmIds.Contains(c.FarmId)).Include(c => c.Farm).Include(c => c.Batch).AsQueryable();
        if (farmId.HasValue) query = query.Where(c => c.FarmId == farmId.Value);
        if (batchId.HasValue) query = query.Where(c => c.BatchId == batchId.Value);
        var costs = await query.OrderByDescending(c => c.CostDate).Take(100).ToListAsync();
        ViewBag.TotalCosts = costs.Sum(c => c.Amount);
        ViewBag.Farms = await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync();
        ViewBag.CategoryDisplayMap = (await db.CostCategoryConfigs.ToListAsync())
            .ToDictionary(c => c.CategoryKey, c => c.DisplayName);
        return View(costs);
    }

    private async Task PopulateCostCategoriesAsync()
    {
        var all = await db.CostCategoryConfigs
            .OrderBy(c => c.ParentId)
            .ThenBy(c => c.SortOrder)
            .ToListAsync();
        ViewBag.CostCategories = all.Where(c => c.ParentId == null).ToList();
        ViewBag.CostSubCategories = all.Where(c => c.ParentId != null).ToList();
    }

    [Authorize(Roles = "SuperAdmin,Manager,Accountant")]
    public async Task<IActionResult> Create(int? batchId, int? cropSeasonId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync();
        ViewBag.Batches = await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync();
        ViewBag.CropSeasons = await db.CropSeasons
            .Where(cs => farmIds.Contains(cs.Land!.FarmId))
            .Include(cs => cs.Land).ThenInclude(l => l!.Farm)
            .ToListAsync();
        await PopulateCostCategoriesAsync();
        // Pre-fill FarmId from the crop season so the JS shows the right dropdowns
        int? preselectedFarmId = null;
        if (cropSeasonId.HasValue)
        {
            var cs = await db.CropSeasons.Include(c => c.Land).FirstOrDefaultAsync(c => c.Id == cropSeasonId.Value);
            preselectedFarmId = cs?.Land?.FarmId;
        }
        return View(new Cost { CostDate = DateTime.Today, BatchId = batchId, CropSeasonId = cropSeasonId, FarmId = preselectedFarmId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Accountant")]
    public async Task<IActionResult> Create(Cost model)
    {
        ModelState.Remove("Farm");
        ModelState.Remove("Batch");
        ModelState.Remove("CropSeason");
        ModelState.Remove("EnteredBy");
        ModelState.Remove("EnteredById");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Farms = await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync();
            ViewBag.Batches = await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync();
            ViewBag.CropSeasons = await db.CropSeasons
                .Where(cs => farmIds.Contains(cs.Land!.FarmId))
                .Include(cs => cs.Land).ThenInclude(l => l!.Farm)
                .ToListAsync();
            await PopulateCostCategoriesAsync();
            return View(model);
        }
        var user = await userManager.GetUserAsync(User);
        model.EnteredById = user?.Id;
        db.Costs.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Cost recorded.";
        if (model.BatchId.HasValue)
            return RedirectToAction("Details", "Batch", new { id = model.BatchId.Value });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var cost = await db.Costs.FindAsync(id);
        if (cost is null) return NotFound();
        cost.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Cost deleted.";
        return RedirectToAction(nameof(Index));
    }
}
