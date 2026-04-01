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
public class SaleController(ApplicationDbContext db, UserManager<AppUser> userManager) : Controller
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
        var query = db.Sales.Where(s => farmIds.Contains(s.Batch!.FarmId)).Include(s => s.Batch).AsQueryable();
        if (batchId.HasValue) query = query.Where(s => s.BatchId == batchId.Value);
        var sales = await query.OrderByDescending(s => s.SaleDate).ToListAsync();
        ViewBag.Batches = await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync();
        return View(sales);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(int? batchId)
    {
        var farmIds = await GetFarmIdsAsync();
        ViewBag.Batches = new SelectList(
            await db.Batches.Where(b => farmIds.Contains(b.FarmId) && (b.Status == BatchStatus.Active || b.Status == BatchStatus.Selling)).ToListAsync(),
            "Id", "BatchName", batchId);
        return View(new Sale { SaleDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create(Sale model)
    {
        ModelState.Remove("Batch");
        if (!ModelState.IsValid)
        {
            var farmIds = await GetFarmIdsAsync();
            ViewBag.Batches = new SelectList(await db.Batches.Where(b => farmIds.Contains(b.FarmId)).ToListAsync(), "Id", "BatchName");
            return View(model);
        }

        model.TotalRevenue = model.TotalWeight_kg.HasValue ? model.TotalWeight_kg.Value * model.PricePerKg : model.TotalRevenue;
        db.Sales.Add(model);

        // Update batch status to Selling
        var batch = await db.Batches.FindAsync(model.BatchId);
        if (batch is not null && batch.Status == BatchStatus.Active)
        {
            batch.Status = BatchStatus.Selling;
        }

        await db.SaveChangesAsync();
        TempData["Success"] = "Sale recorded successfully.";
        return RedirectToAction("Details", "Batch", new { id = model.BatchId });
    }

    public async Task<IActionResult> History(int? farmId)
    {
        var farmIds = await GetFarmIdsAsync();
        if (farmId.HasValue && farmIds.Contains(farmId.Value))
            farmIds = [farmId.Value];

        var sales = await db.Sales
            .Where(s => farmIds.Contains(s.Batch!.FarmId))
            .Include(s => s.Batch).ThenInclude(b => b!.Farm)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        ViewBag.TotalRevenue = sales.Sum(s => s.TotalRevenue);
        return View(sales);
    }
}
