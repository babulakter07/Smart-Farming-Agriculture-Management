using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class WeightLogController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index(int batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        var batch = await db.Batches.Include(b => b.Farm).FirstOrDefaultAsync(b => b.Id == batchId);
        if (batch is null || !farmIds.Contains(batch.FarmId)) return NotFound();

        var logs = await db.WeightLogs.Where(w => w.BatchId == batchId).OrderBy(w => w.LogDate).ToListAsync();
        ViewBag.Batch = batch;
        ViewBag.ChartLabels = logs.Select(l => l.LogDate.ToString("MMM dd")).ToArray();
        ViewBag.ChartData = logs.Select(l => l.AvgWeight_kg).ToArray();
        return View(logs);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(int batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName", batchId);
        return View(new WeightLog { BatchId = batchId, LogDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(WeightLog model)
    {
        ModelState.Remove("Batch");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName");
            return View(model);
        }

        // Compute FCR
        var totalFeedKg = await db.DailyFeedLogs.Where(f => f.BatchId == model.BatchId).SumAsync(f => (decimal?)f.Quantity_kg) ?? 0;
        var batch = await db.Batches.FindAsync(model.BatchId);
        if (batch is not null && model.AvgWeight_kg > 0)
        {
            var initialAvgWeight = batch.InitialWeight_kg.HasValue ? batch.InitialWeight_kg.Value / batch.InitialCount : 0;
            var weightGain = (model.AvgWeight_kg - initialAvgWeight) * batch.InitialCount;
            model.FCR_Cumulative = weightGain > 0 ? Math.Round(totalFeedKg / weightGain, 3) : null;
            model.TotalEstWeight = model.AvgWeight_kg * batch.InitialCount;
        }

        db.WeightLogs.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Weight log added.";
        return RedirectToAction("Details", "Batch", new { id = model.BatchId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await db.WeightLogs.FindAsync(id);
        if (log is null) return NotFound();
        log.IsDeleted = true;
        await db.SaveChangesAsync();
        return RedirectToAction("Details", "Batch", new { id = log.BatchId });
    }
}
