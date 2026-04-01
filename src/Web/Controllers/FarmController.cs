using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class FarmController(FarmService farmService) : Controller
{
    private string UserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var farms = await farmService.GetAllAsync(UserId, IsSuperAdmin, ct);
        return View(farms);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var farm = await farmService.GetByIdAsync(id, ct);
        if (farm == null) return NotFound();
        return View(farm);
    }

    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public IActionResult Create() => View(new FarmCreateDto());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Create(FarmCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        var id = await farmService.CreateAsync(dto, UserId, ct);
        TempData["Success"] = "Farm created successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var farm = await farmService.GetByIdAsync(id, ct);
        if (farm == null) return NotFound();
        var dto = new FarmEditDto
        {
            Id = farm.Id,
            FarmName = farm.FarmName,
            FarmType = farm.FarmType,
            Location = farm.Location,
            Latitude = farm.Latitude,
            Longitude = farm.Longitude,
            TotalArea = farm.TotalArea,
            IsActive = farm.IsActive
        };
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,FarmManager")]
    public async Task<IActionResult> Edit(FarmEditDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await farmService.UpdateAsync(dto, UserId, ct);
        TempData["Success"] = "Farm updated successfully.";
        return RedirectToAction(nameof(Details), new { id = dto.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await farmService.DeleteAsync(id, UserId, ct);
        TempData["Success"] = "Farm deleted.";
        return RedirectToAction(nameof(Index));
    }
}
