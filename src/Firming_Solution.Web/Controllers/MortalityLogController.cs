using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class MortalityLogController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var query = db.MortalityLogs
            .Where(m => farmIds.Contains(m.Batch!.FarmId))
            .Include(m => m.Batch)
            .AsQueryable();
        if (batchId.HasValue) query = query.Where(m => m.BatchId == batchId.Value);
        var logs = await query.OrderByDescending(m => m.LogDate).Take(100).ToListAsync();
        ViewBag.Batches = await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync();
        ViewBag.SelectedBatchId = batchId;
        return View(logs);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Batches = new SelectList(
            await db.Batches.Where(b => farmIds.Contains(b.FarmId) && b.Status == Domain.Enums.BatchStatus.Active).ToListAsync(),
            "Id", "BatchName", batchId);
        return View(new MortalityLog { LogDate = DateTime.Today, BatchId = batchId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(MortalityLog model)
    {
        ModelState.Remove("Batch");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Batches = new SelectList(
                await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(),
                "Id", "BatchName");
            return View(model);
        }
        db.MortalityLogs.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Mortality log added.";
        return RedirectToAction("Details", "Batch", new { id = model.BatchId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await db.MortalityLogs.FindAsync(id);
        if (log is null) return NotFound();
        log.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Mortality log deleted.";
        return RedirectToAction("Details", "Batch", new { id = log.BatchId });
    }
}
