using Firming_Solution.Domain.Entities;
using Firming_Solution.Domain.Enums;
using Firming_Solution.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firming_Solution.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class AdminController(
    UserManager<AppUser> userManager,
    ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Users()
    {
        var users = await userManager.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToListAsync();
        return View(users);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        user.IsActive = !user.IsActive;
        await userManager.UpdateAsync(user);
        TempData["Success"] = $"User '{user.FullName ?? user.UserName}' {(user.IsActive ? "activated" : "deactivated")}.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string userId, UserRole role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        // Remove all roles, then add new one
        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        user.Role = role;
        await userManager.UpdateAsync(user);
        await userManager.AddToRoleAsync(user, role.ToString());

        TempData["Success"] = $"Role for '{user.FullName ?? user.UserName}' changed to {role}.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        user.IsDeleted = true;
        user.IsActive = false;
        await userManager.UpdateAsync(user);
        TempData["Success"] = "User deleted (soft delete).";
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> AssignFarm()
    {
        var users = await userManager.Users.Where(u => !u.IsDeleted && u.IsActive).ToListAsync();
        var farms = await db.Farms.ToListAsync();
        ViewBag.Users = users;
        ViewBag.Farms = farms;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignFarm(string userId, int farmId)
    {
        var existing = await db.UserFarms.FirstOrDefaultAsync(uf => uf.UserId == userId && uf.FarmId == farmId);
        if (existing is null)
        {
            db.UserFarms.Add(new UserFarm { UserId = userId, FarmId = farmId });
            await db.SaveChangesAsync();
            TempData["Success"] = "Farm assigned to user.";
        }
        else
        {
            TempData["Error"] = "User already has access to this farm.";
        }
        return RedirectToAction(nameof(Users));
    }
}
