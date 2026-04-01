using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class InvestmentController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var investments = await db.Investments
            .Where(i => farmIds.Contains(i.FarmId))
            .Include(i => i.Farm)
            .Include(i => i.RecordedBy)
            .OrderByDescending(i => i.InvestDate)
            .ToListAsync();
        ViewBag.TotalInvestment = investments.Sum(i => i.Amount);
        return View(investments);
    }

    [Authorize(Roles = "SuperAdmin,Manager,Accountant")]
    public async Task<IActionResult> Create()
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
        return View(new Investment { InvestDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager,Accountant")]
    public async Task<IActionResult> Create(Investment model)
    {
        ModelState.Remove("Farm");
        ModelState.Remove("RecordedBy");
        ModelState.Remove("RecordedById");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Farms = new SelectList(await db.Farms.Where(f => farmIds.Contains(f.Id)).ToListAsync(), "Id", "FarmName");
            return View(model);
        }
        var user = await userManager.GetUserAsync(User);
        model.RecordedById = user?.Id;
        db.Investments.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Investment logged.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var inv = await db.Investments.FindAsync(id);
        if (inv is null) return NotFound();
        inv.IsDeleted = true;
        await db.SaveChangesAsync();
        TempData["Success"] = "Investment deleted.";
        return RedirectToAction(nameof(Index));
    }
}
