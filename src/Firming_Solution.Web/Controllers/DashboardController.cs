using Firming_Solution.Application.Services;
using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class DashboardController(
    DashboardService dashboardService,
    UserManager<AppUser> userManager,
    ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction("Login", "Account");

        bool isSuperAdmin = User.IsInRole("SuperAdmin");
        var stats = await dashboardService.GetStatsAsync(user.Id, isSuperAdmin);

        // Recent activity
        IQueryable<int> farmIdsQ = isSuperAdmin
            ? db.Farms.Select(f => f.Id)
            : db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId);

        var farmIds = await farmIdsQ.ToListAsync();

        var recentBatches = await db.Batches
            .Where(b => farmIds.Contains(b.FarmId))
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Include(b => b.Farm)
            .ToListAsync();

        var todayTasks = await db.DailyTasks
            .Where(t => farmIds.Contains(t.FarmId) && t.TaskDate == DateTime.Today)
            .Include(t => t.AssignedTo)
            .OrderBy(t => t.StartTime)
            .Take(10)
            .ToListAsync();

        var recentSales = await db.Sales
            .Where(s => farmIds.Contains(s.Batch!.FarmId))
            .OrderByDescending(s => s.SaleDate)
            .Take(5)
            .Include(s => s.Batch)
            .ToListAsync();

        var eidPlans = await db.EidBazarPlans
            .Where(e => farmIds.Contains(e.FarmId) && e.EidDate >= DateTime.Today)
            .Include(e => e.LinkedBatch)
            .OrderBy(e => e.EidDate)
            .Take(3)
            .ToListAsync();

        ViewBag.Stats = stats;
        ViewBag.RecentBatches = recentBatches;
        ViewBag.TodayTasks = todayTasks;
        ViewBag.RecentSales = recentSales;
        ViewBag.EidPlans = eidPlans;
        ViewBag.UserName = user.FullName ?? user.UserName;
        ViewBag.UserRole = user.Role.ToString();

        return View();
    }
}
