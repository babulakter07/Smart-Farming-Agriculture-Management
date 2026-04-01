using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class WeatherLogController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var logs = await db.WeatherLogs
            .Where(w => farmIds.Contains(w.FarmId))
            .Include(w => w.Farm)
            .OrderByDescending(w => w.LogDate)
            .Take(100)
            .ToListAsync();
        return View(logs);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        return View(new WeatherLog { LogDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Worker")]
    public async Task<IActionResult> Create(WeatherLog model)
    {
        ModelState.Remove("Farm");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            return View(model);
        }
        db.WeatherLogs.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Weather log recorded.";
        return RedirectToAction(nameof(Index));
    }
}
