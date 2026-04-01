using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin,FarmManager")]
public class LandController(LandService landService) : Controller
{
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

    public async Task<IActionResult> Index(int farmId, CancellationToken ct)
    {
        var lands = await landService.GetByFarmAsync(farmId, ct);
        ViewBag.FarmId = farmId;
        return View(lands);
    }

    public IActionResult Create(int farmId)
    {
        LoadOwnershipDropdown();
        return View(new LandParcelCreateDto { FarmId = farmId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LandParcelCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            LoadOwnershipDropdown();
            return View(dto);
        }
        await landService.CreateLandAsync(dto, UserId, ct);
        TempData["Success"] = "Land parcel added.";
        return RedirectToAction(nameof(Index), new { farmId = dto.FarmId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await landService.GetLandByIdAsync(id, ct);
        if (dto == null) return NotFound();
        LoadOwnershipDropdown();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LandParcelEditDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            LoadOwnershipDropdown();
            return View(dto);
        }
        await landService.UpdateLandAsync(dto, UserId, ct);
        TempData["Success"] = "Land parcel updated.";
        return RedirectToAction(nameof(Index), new { farmId = dto.FarmId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int farmId, CancellationToken ct)
    {
        await landService.DeleteLandAsync(id, UserId, ct);
        TempData["Success"] = "Land parcel deleted.";
        return RedirectToAction(nameof(Index), new { farmId });
    }

    // Crop Seasons
    public async Task<IActionResult> CropSeasons(int landId, CancellationToken ct)
    {
        var seasons = await landService.GetCropSeasonsByLandAsync(landId, ct);
        ViewBag.LandId = landId;
        return View(seasons);
    }

    public IActionResult CreateCropSeason(int landId)
    {
        LoadCropStatusDropdown();
        return View(new CropSeasonCreateDto { LandId = landId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCropSeason(CropSeasonCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            LoadCropStatusDropdown();
            return View(dto);
        }
        await landService.CreateCropSeasonAsync(dto, UserId, ct);
        TempData["Success"] = "Crop season created.";
        return RedirectToAction(nameof(CropSeasons), new { landId = dto.LandId });
    }

    private void LoadOwnershipDropdown()
    {
        ViewBag.OwnershipList = Enum.GetValues<OwnershipType>()
            .Select(o => new SelectListItem(o.ToString(), o.ToString())).ToList();
    }

    private void LoadCropStatusDropdown()
    {
        ViewBag.StatusList = Enum.GetValues<CropSeasonStatus>()
            .Select(s => new SelectListItem(s.ToString(), s.ToString())).ToList();
    }
}
