using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class FeedLogController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index(int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        var query = db.DailyFeedLogs
            .Where(f => farmIds.Contains(f.Batch!.FarmId))
            .Include(f => f.Batch).Include(f => f.FeedType).Include(f => f.LoggedBy)
            .AsQueryable();
        if (batchId.HasValue) query = query.Where(f => f.BatchId == batchId.Value);
        var logs = await query.OrderByDescending(f => f.LogDate).Take(100).ToListAsync();
        ViewBag.Batches = await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync();
        ViewBag.SelectedBatchId = batchId;
        return View(logs);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId) && b.Status == Domain.Enums.BatchStatus.Active).ToListAsync(), "Id", "BatchName", batchId);
        ViewBag.FeedTypes = new SelectList(await db.FeedTypes.ToListAsync(), "Id", "FeedName");
        return View(new DailyFeedLog { LogDate = DateTime.Today, Quantity_kg = 1, PricePerKg = 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(DailyFeedLog model)
    {
        ModelState.Remove("Batch");
        ModelState.Remove("FeedType");
        ModelState.Remove("LoggedBy");
        ModelState.Remove("LoggedById");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName");
            ViewBag.FeedTypes = new SelectList(await db.FeedTypes.ToListAsync(), "Id", "FeedName");
            return View(model);
        }
        var user = await userManager.GetUserAsync(User);
        model.LoggedById = user?.Id;
        db.DailyFeedLogs.Add(model);
        await db.SaveChangesAsync();

        // Update feed type price
        var feedType = await db.FeedTypes.FindAsync(model.FeedTypeId);
        if (feedType is not null)
        {
            feedType.CurrentPrice = model.PricePerKg;
            feedType.LastUpdated = DateTime.UtcNow;
            db.FeedCostHistories.Add(new FeedCostHistory { FeedTypeId = model.FeedTypeId, RecordedDate = model.LogDate, PricePerKg = model.PricePerKg });
            await db.SaveChangesAsync();
        }

        TempData["Success"] = "Feed log added.";
        return RedirectToAction("Details", "Batch", new { id = model.BatchId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await db.DailyFeedLogs.FindAsync(id);
        if (log is null) return NotFound();
        log.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Feed log deleted.";
        return RedirectToAction("Details", "Batch", new { id = log.BatchId });
    }
}
