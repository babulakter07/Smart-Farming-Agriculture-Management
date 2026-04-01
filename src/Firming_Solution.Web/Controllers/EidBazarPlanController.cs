using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class EidBazarPlanController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var plans = await db.EidBazarPlans
            .Where(e => farmIds.Contains(e.FarmId))
            .Include(e => e.Farm)
            .Include(e => e.LinkedBatch)
            .OrderBy(e => e.EidDate)
            .ToListAsync();
        return View(plans);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        ViewBag.Batches = new SelectList(
            await db.Batches.Where(b => farmIds.Contains(b.FarmId) && b.IsEidTarget).ToListAsync(),
            "Id", "BatchName");
        return View(new EidBazarPlan { EidDate = DateTime.Today.AddDays(90) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(EidBazarPlan model)
    {
        ModelState.Remove("Farm");
        ModelState.Remove("LinkedBatch");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            ViewBag.Batches = new SelectList(
                await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(),
                "Id", "BatchName");
            return View(model);
        }
        db.EidBazarPlans.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Eid Bazar plan created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await db.EidBazarPlans.FindAsync(id);
        if (plan is null) return NotFound();
        plan.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Plan deleted.";
        return RedirectToAction(nameof(Index));
    }
}
