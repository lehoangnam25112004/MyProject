using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class ProgressController(
    IProgressService progressService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var tasks = await progressService.GetVisibleTasksAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return View(new ProgressIndexViewModel { Tasks = tasks });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        ProgressUpdateViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Dữ liệu tiến độ không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await progressService.UpdateAsync(
            model.TaskId,
            model.PercentComplete,
            model.Status,
            model.DelayReason,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã cập nhật tiến độ." : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ChartData(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var data = await progressService.GetChartAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return Json(data);
    }
}
