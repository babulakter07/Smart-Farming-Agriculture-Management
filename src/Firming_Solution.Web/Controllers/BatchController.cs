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
public class BatchController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index(int? farmId)
    {
        var farmIds = await GetFarmIdsAsync();
        var query = db.Batches.Where(b => farmIds.Contains(b.FarmId)).Include(b => b.Farm).AsQueryable();
        if (farmId.HasValue) query = query.Where(b => b.FarmId == farmId.Value);
        var batches = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        ViewBag.Farms = await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync();
        ViewBag.SelectedFarmId = farmId;
        return View(batches);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        return View(new Batch { StartDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(Batch model)
    {
        ModelState.Remove("Farm");
        if (!ModelState.IsValid)
        {
            var fids = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => fids.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            return View(model);
        }
        db.Batches.Add(model);
        await db.SaveChangesAsync();

        // Record purchase cost as a cost entry
        db.Costs.Add(new Cost
        {
            BatchId = model.Id,
            FarmId = model.FarmId,
            CostCategory = CostCategory.Other,
            Description = "Animal Purchase Cost",
            CostDate = model.StartDate,
            Amount = model.PurchaseCost,
            IsActual = true
        });
        await db.SaveChangesAsync();

        TempData["Success"] = "Batch created successfully.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var farmIds = await GetFarmIdsAsync();
        var batch = await db.Batches
            .Include(b => b.Farm)
            .Include(b => b.FeedLogs).ThenInclude(f => f.FeedType)
            .Include(b => b.WeightLogs)
            .Include(b => b.MortalityLogs)
            .Include(b => b.Sales)
            .Include(b => b.Costs)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (batch is null || !farmIds.Contains(batch.FarmId)) return NotFound();

        var totalFeedCost = batch.FeedLogs.Sum(f => f.Quantity_kg * f.PricePerKg);
        // totalCosts already includes PurchaseCost (auto-inserted as a Cost record on batch creation)
        var totalCosts = batch.Costs.Sum(c => c.Amount);
        var totalAllCosts = totalFeedCost + totalCosts;   // true total spend
        var totalRevenue = batch.Sales.Sum(s => s.TotalRevenue);
        var totalDeaths = batch.MortalityLogs.Sum(m => m.Count);
        var liveCount = batch.InitialCount - totalDeaths;

        ViewBag.TotalFeedCost = totalFeedCost;
        ViewBag.TotalCosts = totalAllCosts;        // full cost shown in stat card
        ViewBag.OtherCosts = totalCosts - batch.PurchaseCost;  // non-purchase, non-feed costs
        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.LiveCount = liveCount;
        ViewBag.GrossProfit = totalRevenue - totalAllCosts;

        return View(batch);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var farmIds = await GetFarmIdsAsync();
        var batch = await db.Batches.FindAsync(id);
        if (batch is null || !farmIds.Contains(batch.FarmId)) return NotFound();
        var fids = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => fids.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        return View(batch);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Edit(int id, Batch model)
    {
        ModelState.Remove("Farm");
        var batch = await db.Batches.FindAsync(id);
        if (batch is null) return NotFound();

        if (!ModelState.IsValid)
        {
            var fids = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => fids.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            return View(model);
        }

        batch.BatchName = model.BatchName;
        batch.Species = model.Species;
        batch.Breed = model.Breed;
        batch.PlannedEndDate = model.PlannedEndDate;
        batch.Status = model.Status;
        batch.IsEidTarget = model.IsEidTarget;
        batch.Notes = model.Notes;
        await db.SaveChangesAsync();

        TempData["Success"] = "Batch updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> UpdateStatus(int id, BatchStatus status)
    {
        var batch = await db.Batches.FindAsync(id);
        if (batch is null) return NotFound();
        batch.Status = status;
        await db.SaveChangesAsync();
        TempData["Success"] = $"Batch status changed to {status}.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var batch = await db.Batches.FindAsync(id);
        if (batch is null) return NotFound();
        batch.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Batch deleted.";
        return RedirectToAction(nameof(Index));
    }
}
