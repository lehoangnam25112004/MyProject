using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class WbsController(
    IWbsService wbsService,
    IProjectService projectService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(int? projectId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var isAdmin = User.IsInRole("Admin") || User.IsInRole("Project Director");
        if (!projectId.HasValue)
        {
            var visibleProjects = await projectService.GetVisibleProjectsAsync(
                user.Id,
                isAdmin,
                cancellationToken);
            var firstProject = visibleProjects.FirstOrDefault();
            return firstProject is null
                ? RedirectToAction("Index", "Projects")
                : RedirectToAction(nameof(Index), new { projectId = firstProject.Id });
        }

        var project = await wbsService.GetProjectTreeAsync(
            projectId.Value,
            user.Id,
            isAdmin,
            cancellationToken);
        return project is null ? NotFound() : View(new WbsIndexViewModel { Project = project });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> CreatePhase(int projectId, CancellationToken cancellationToken)
    {
        await EnsureProjectAsync(projectId, cancellationToken);
        return View(new ProjectPhaseViewModel { ProjectId = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> CreatePhase(
        ProjectPhaseViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await wbsService.CreatePhaseAsync(
            new ProjectPhase
            {
                ProjectId = model.ProjectId,
                Name = model.Name,
                SequenceOrder = model.SequenceOrder
            },
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> CreateItem(int projectId, CancellationToken cancellationToken)
    {
        var project = await wbsService.GetProjectTreeAsync(
            projectId,
            (await userManager.GetUserAsync(User))?.Id ?? string.Empty,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        ViewBag.Phases = project.Phases;
        ViewBag.WorkItems = project.Phases.SelectMany(phase => phase.WorkItems).ToList();
        return View(new WorkItemViewModel { ProjectId = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> CreateItem(
        WorkItemViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadItemOptionsAsync(model.ProjectId, cancellationToken);
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await wbsService.CreateWorkItemAsync(
            new WorkItem
            {
                ProjectId = model.ProjectId,
                ProjectPhaseId = model.ProjectPhaseId,
                ParentWorkItemId = model.ParentWorkItemId,
                WbsCode = model.WbsCode,
                Name = model.Name,
                SequenceOrder = model.SequenceOrder,
                Status = model.Status
            },
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadItemOptionsAsync(model.ProjectId, cancellationToken);
            return View(model);
        }

        return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
    }

    private async Task LoadItemOptionsAsync(int projectId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        var project = await wbsService.GetProjectTreeAsync(
            projectId,
            user?.Id ?? string.Empty,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        ViewBag.Phases = project?.Phases ?? [];
        ViewBag.WorkItems = project?.Phases.SelectMany(phase => phase.WorkItems).ToList() ?? [];
    }

    private async Task EnsureProjectAsync(int projectId, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        var project = await projectService.GetByIdAsync(
            projectId,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (project is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }
}
