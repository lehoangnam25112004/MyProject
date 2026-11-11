using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskStatus = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Controllers;

[Authorize(Roles = "Admin,Project Director,Project Manager,Site Manager,Supervising Engineer")]
public class QualityController(
    ApplicationDbContext dbContext,
    IQualityService qualityService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(new QualityIndexViewModel
        {
            Incidents = await dbContext.Incidents.AsNoTracking()
                .Include(item => item.Project).Include(item => item.AssignedTo)
                .OrderByDescending(item => item.CreatedAt).Take(100).ToListAsync(cancellationToken),
            Rfis = await dbContext.RfiRequests.AsNoTracking()
                .Include(item => item.Project).Include(item => item.CreatedBy)
                .OrderByDescending(item => item.CreatedAt).Take(100).ToListAsync(cancellationToken),
            Inspections = await dbContext.InspectionRequests.AsNoTracking()
                .Include(item => item.Project).Include(item => item.Task)
                .Include(item => item.InspectedBy)
                .OrderByDescending(item => item.RequestedAt).Take(100).ToListAsync(cancellationToken)
        });
    }

    public async Task<IActionResult> CreateIncident(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        return View(new IncidentCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateIncident(
        IncidentCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await qualityService.CreateIncidentAsync(model, userId, cancellationToken);
        return await HandleResult(result, nameof(CreateIncident), model, cancellationToken);
    }

    public async Task<IActionResult> CreateRfi(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        return View(new RfiCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRfi(
        RfiCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await qualityService.CreateRfiAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã tạo RFI.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult RespondRfi(int id) => View(new RfiRespondViewModel { });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RespondRfi(
        int id, RfiRespondViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await qualityService.RespondRfiAsync(id, model, userId, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã ghi nhận phản hồi RFI." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> CreateInspection(CancellationToken cancellationToken)
    {
        ViewBag.Tasks = await dbContext.Tasks
            .Include(task => task.Project)
            .Where(task => task.RequiresInspection && task.Status != TaskStatus.Cancelled)
            .OrderBy(task => task.Project.ProjectCode).ThenBy(task => task.Title)
            .ToListAsync(cancellationToken);
        return View(new InspectionCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInspection(
        InspectionCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadInspectionTasksAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await qualityService.CreateInspectionAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadInspectionTasksAsync(cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã tạo yêu cầu nghiệm thu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordInspection(
        int id, InspectionResultViewModel model, CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await qualityService.RecordInspectionAsync(id, model, userId, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã ghi nhận kết quả nghiệm thu." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> HandleResult(
        (bool Succeeded, string? Error) result, string action,
        IncidentCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(action, model);
        }
        TempData["SuccessMessage"] = "Đã tạo sự cố.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Projects = await dbContext.Projects.Where(project => !project.IsArchived)
            .OrderBy(project => project.ProjectCode).ToListAsync(cancellationToken);
        ViewBag.Tasks = await dbContext.Tasks.Include(task => task.Project)
            .Where(task => task.Status != TaskStatus.Cancelled)
            .OrderBy(task => task.Title).ToListAsync(cancellationToken);
        ViewBag.Users = await dbContext.Users.Where(user => user.IsActive)
            .OrderBy(user => user.FullName).ToListAsync(cancellationToken);
    }

    private async Task LoadInspectionTasksAsync(CancellationToken cancellationToken)
    {
        ViewBag.Tasks = await dbContext.Tasks.Include(task => task.Project)
            .Where(task => task.RequiresInspection && task.Status != TaskStatus.Cancelled)
            .OrderBy(task => task.Title).ToListAsync(cancellationToken);
    }
}
