using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class BatchController(BatchService batchService, FarmService farmService) : Controller
{
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");

    public async Task<IActionResult> Index(int farmId, CancellationToken ct)
    {
        var batches = await batchService.GetByFarmAsync(farmId, ct);
        ViewBag.FarmId = farmId;
        var farm = await farmService.GetByIdAsync(farmId, ct);
        ViewBag.FarmName = farm?.FarmName;
        return View(batches);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var batch = await batchService.GetByIdAsync(id, ct);
        if (batch == null) return NotFound();
        return View(batch);
    }

    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Create(int farmId, CancellationToken ct)
    {
        ViewBag.FarmId = farmId;
        await LoadDropdowns(ct);
        return View(new BatchCreateDto { FarmId = farmId, StartDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Create(BatchCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(ct);
            return View(dto);
        }
        var id = await batchService.CreateAsync(dto, UserId, ct);
        TempData["Success"] = "Batch created successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var batch = await batchService.GetByIdAsync(id, ct);
        if (batch == null) return NotFound();
        await LoadDropdowns(ct);
        var dto = new BatchEditDto
        {
            Id = batch.Id,
            FarmId = batch.FarmId,
            BatchName = batch.BatchName,
            Species = batch.Species,
            Breed = batch.Breed,
            StartDate = batch.StartDate,
            PlannedEndDate = batch.PlannedEndDate,
            InitialCount = batch.InitialCount,
            InitialWeight_kg = batch.InitialWeight_kg,
            PurchaseCost = batch.PurchaseCost,
            Status = batch.Status,
            IsEidTarget = batch.IsEidTarget,
            Notes = batch.Notes
        };
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Edit(BatchEditDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(ct);
            return View(dto);
        }
        await batchService.UpdateAsync(dto, UserId, ct);
        TempData["Success"] = "Batch updated.";
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Delete(int id, int farmId, CancellationToken ct)
    {
        await batchService.DeleteAsync(id, UserId, ct);
        TempData["Success"] = "Batch deleted.";
        return RedirectToAction(nameof(Index), new { farmId });
    }

    private async Task LoadDropdowns(CancellationToken ct)
    {
        ViewBag.SpeciesList = Enum.GetValues<BatchSpecies>().Select(s => new SelectListItem(s.ToString(), s.ToString())).ToList();
        ViewBag.StatusList = Enum.GetValues<BatchStatus>().Select(s => new SelectListItem(s.ToString(), s.ToString())).ToList();
        await Task.CompletedTask;
    }
}
