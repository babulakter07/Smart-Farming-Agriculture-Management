using Firming_Solution.Domain.Entities;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class SettingsController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> CostCategories()
    {
        var all = await db.CostCategoryConfigs
            .OrderBy(c => c.ParentId)
            .ThenBy(c => c.SortOrder)
            .ToListAsync();

        var parents = all.Where(c => c.ParentId == null).ToList();
        foreach (var p in parents)
            p.SubCategories = all.Where(c => c.ParentId == p.Id).ToList();

        return View(parents);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string categoryKey, string displayName, int? parentId)
    {
        if (string.IsNullOrWhiteSpace(categoryKey) || string.IsNullOrWhiteSpace(displayName))
        {
            TempData["Error"] = "নাম ও কী পূরণ করুন।";
            return RedirectToAction(nameof(CostCategories));
        }

        var maxOrder = await db.CostCategoryConfigs
            .Where(c => c.ParentId == parentId)
            .Select(c => (int?)c.SortOrder)
            .MaxAsync() ?? 0;

        db.CostCategoryConfigs.Add(new CostCategoryConfig
        {
            CategoryKey = categoryKey.Trim(),
            DisplayName = displayName.Trim(),
            ParentId = parentId,
            SortOrder = maxOrder + 1
        });
        await db.SaveChangesAsync();
        TempData["Success"] = "বিভাগ যোগ করা হয়েছে।";
        return RedirectToAction(nameof(CostCategories));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, string displayName)
    {
        var cat = await db.CostCategoryConfigs.FindAsync(id);
        if (cat is null) return NotFound();

        cat.DisplayName = displayName.Trim();
        await db.SaveChangesAsync();
        TempData["Success"] = "বিভাগ আপডেট হয়েছে।";
        return RedirectToAction(nameof(CostCategories));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var cat = await db.CostCategoryConfigs
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (cat is null) return NotFound();

        // soft-delete subcategories too
        foreach (var sub in cat.SubCategories)
            sub.IsDeleted = true;
        cat.IsDeleted = true;

        await db.SaveChangesAsync();
        TempData["Success"] = "বিভাগ মুছে ফেলা হয়েছে।";
        return RedirectToAction(nameof(CostCategories));
    }
}
