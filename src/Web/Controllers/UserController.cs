using Firming_Solution.Application.DTOs;
using Firming_Solution.Application.Services;
using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class UserController(UserService userService, FarmService farmService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await userService.GetAllAsync(ct);
        return View(users);
    }

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await LoadFarmsDropdown(ct);
        return View(new UserCreateDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadFarmsDropdown(ct);
            return View(dto);
        }
        var (success, errors) = await userService.CreateAsync(dto);
        if (!success)
        {
            foreach (var e in errors) ModelState.AddModelError("", e);
            await LoadFarmsDropdown(ct);
            return View(dto);
        }
        TempData["Success"] = "User created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id, CancellationToken ct)
    {
        var dto = await userService.GetEditDtoAsync(id, ct);
        if (dto == null) return NotFound();
        await LoadFarmsDropdown(ct);
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadFarmsDropdown(ct);
            return View(dto);
        }
        var (success, errors) = await userService.UpdateAsync(dto);
        if (!success)
        {
            foreach (var e in errors) ModelState.AddModelError("", e);
            await LoadFarmsDropdown(ct);
            return View(dto);
        }
        TempData["Success"] = "User updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await userService.SoftDeleteAsync(id);
        TempData["Success"] = "User deactivated.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadFarmsDropdown(CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var farms = await farmService.GetAllAsync(userId, true, ct);
        ViewBag.FarmList = farms.Select(f => new SelectListItem(f.FarmName, f.Id.ToString())).ToList();
        ViewBag.RoleList = Enum.GetValues<UserRole>().Select(r => new SelectListItem(r.ToString(), r.ToString())).ToList();
    }
}
