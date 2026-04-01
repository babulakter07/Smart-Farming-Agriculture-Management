using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin,FarmManager,Accountant")]
public class CostController(CostService costService) : Controller
{
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

    public async Task<IActionResult> Index(int farmId, CancellationToken ct)
    {
        var costs = await costService.GetByFarmAsync(farmId, null, null, ct);
        ViewBag.FarmId = farmId;
        return View(costs);
    }

    public IActionResult Create(int farmId, int? batchId)
    {
        LoadCategoryDropdown();
        return View(new CostCreateDto { FarmId = farmId, BatchId = batchId, CostDate = DateTime.Today, IsActual = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CostCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            LoadCategoryDropdown();
            return View(dto);
        }
        await costService.CreateAsync(dto, UserId, ct);
        TempData["Success"] = "Cost entry added.";
        return RedirectToAction(nameof(Index), new { farmId = dto.FarmId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await costService.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        LoadCategoryDropdown();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CostEditDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            LoadCategoryDropdown();
            return View(dto);
        }
        await costService.UpdateAsync(dto, UserId, ct);
        TempData["Success"] = "Cost updated.";
        return RedirectToAction(nameof(Index), new { farmId = dto.FarmId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int farmId, CancellationToken ct)
    {
        await costService.DeleteAsync(id, UserId, ct);
        TempData["Success"] = "Cost deleted.";
        return RedirectToAction(nameof(Index), new { farmId });
    }

    private void LoadCategoryDropdown()
    {
        ViewBag.CategoryList = Enum.GetValues<CostCategory>()
            .Select(c => new SelectListItem(c.ToString(), c.ToString())).ToList();
    }
}
