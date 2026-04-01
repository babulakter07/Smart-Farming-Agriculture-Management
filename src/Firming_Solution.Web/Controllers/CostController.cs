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
        return View(costs);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Accountant")]
    public async Task<IActionResult> Create(int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName", batchId);
        return View(new Cost { CostDate = DateTime.Today });
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
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName");
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
