using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin,FarmManager,Accountant")]
public class SaleController(SaleService saleService) : Controller
{
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

    public async Task<IActionResult> Index(int batchId, CancellationToken ct)
    {
        var sales = await saleService.GetByBatchAsync(batchId, ct);
        ViewBag.BatchId = batchId;
        return View(sales);
    }

    public IActionResult Create(int batchId)
    {
        return View(new SaleCreateDto { BatchId = batchId, SaleDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await saleService.CreateAsync(dto, UserId, ct);
        TempData["Success"] = "Sale recorded.";
        return RedirectToAction("Details", "Batch", new { id = dto.BatchId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int batchId, CancellationToken ct)
    {
        await saleService.DeleteAsync(id, UserId, ct);
        TempData["Success"] = "Sale deleted.";
        return RedirectToAction("Details", "Batch", new { id = batchId });
    }
}
