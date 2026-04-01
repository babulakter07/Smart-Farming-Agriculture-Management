using Firming_Solution.Domain.Entities;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using TaskStatus = Firming_Solution.Domain.Enums.TaskStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class DailyTaskController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
{
    private async Task<List<int>> GetFarmIdsAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return [];
        if (User.IsInRole("SuperAdmin"))
            return await db.Farms.Select(f => f.Id).ToListAsync();
        return await db.UserFarms.Where(uf => uf.UserId == user.Id).Select(uf => uf.FarmId).ToListAsync();
    }

    public async Task<IActionResult> Index(DateTime? date)
    {
        var farmIds = await GetFarmIdsAsync();
        var targetDate = date ?? DateTime.Today;
        var tasks = await db.DailyTasks
            .Where(t => farmIds.Contains(t.FarmId) && t.TaskDate == targetDate)
            .Include(t => t.Farm)
            .Include(t => t.AssignedTo)
            .OrderBy(t => t.StartTime)
            .ToListAsync();
        ViewBag.SelectedDate = targetDate;
        ViewBag.Farms = await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync();
        return View(tasks);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        ViewBag.Users = new SelectList(await db.Users.Where(u => u.IsActive).ToListAsync(), "Id", "FullName");
        return View(new DailyTask { TaskDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(DailyTask model)
    {
        ModelState.Remove("Farm");
        ModelState.Remove("AssignedTo");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            ViewBag.Users = new SelectList(await db.Users.Where(u => u.IsActive).ToListAsync(), "Id", "FullName");
            return View(model);
        }
        db.DailyTasks.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Task created.";
        return RedirectToAction(nameof(Index), new { date = model.TaskDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, TaskStatus status)
    {
        var task = await db.DailyTasks.FindAsync(id);
        if (task is null) return NotFound();
        task.Status = status;
        if (status == TaskStatus.Done) task.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        TempData["Success"] = $"Task marked as {status}.";
        return RedirectToAction(nameof(Index), new { date = task.TaskDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await db.DailyTasks.FindAsync(id);
        if (task is null) return NotFound();
        task.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Task deleted.";
        return RedirectToAction(nameof(Index));
    }
}
