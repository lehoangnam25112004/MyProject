using ConstructionProjectManagement.Services;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class DashboardController(
    DashboardService dashboardService,
    UserManager<ConstructionProjectManagement.Models.ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        return View(await dashboardService.GetSummaryAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken));
    }
}
