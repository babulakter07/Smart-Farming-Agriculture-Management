using Firming_Solution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firming_Solution.Web.Controllers;

[Authorize]
public class DashboardController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        var isSuperAdmin = User.IsInRole("SuperAdmin");
        var dashboard = await dashboardService.GetDashboardAsync(userId, isSuperAdmin, ct);
        return View(dashboard);
    }
}
