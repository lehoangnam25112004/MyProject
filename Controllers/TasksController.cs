using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskEntity = ConstructionProjectManagement.Models.Task;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class TasksController(
    ITaskService taskService,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var tasks = await taskService.GetVisibleAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return View(new TaskListViewModel { Tasks = tasks });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var task = await taskService.GetByIdAsync(
            id,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return task is null ? NotFound() : View(new TaskDetailsViewModel { Task = task });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        return View(new TaskEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Create(TaskEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await taskService.CreateAsync(
            ToEntity(model),
            model.AssigneeIds,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo công việc.";
        return RedirectToAction(nameof(Details), new { id = result.Task!.Id });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        var task = await taskService.GetByIdAsync(
            id,
            user?.Id ?? string.Empty,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (task is null)
        {
            return NotFound();
        }

        await LoadOptionsAsync(cancellationToken);
        return View(ToViewModel(task));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Edit(
        int id,
        TaskEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await taskService.UpdateAsync(
            ToEntity(model, id),
            model.AssigneeIds,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Projects = await dbContext.Projects
            .AsNoTracking()
            .Where(project => !project.IsArchived)
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);
        ViewBag.WorkItems = await dbContext.WorkItems
            .AsNoTracking()
            .OrderBy(item => item.WbsCode)
            .ToListAsync(cancellationToken);
        ViewBag.Tasks = await dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Title)
            .ToListAsync(cancellationToken);
        ViewBag.Users = await dbContext.Users
            .AsNoTracking()
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);
    }

    private static TaskEntity ToEntity(TaskEditViewModel model, int? id = null)
    {
        return new TaskEntity
        {
            Id = id ?? 0,
            ProjectId = model.ProjectId,
            WorkItemId = model.WorkItemId,
            ParentTaskId = model.ParentTaskId,
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            PlannedStartDate = model.PlannedStartDate,
            PlannedEndDate = model.PlannedEndDate,
            ActualStartDate = model.ActualStartDate,
            ActualEndDate = model.ActualEndDate,
            Status = model.Status,
            PercentComplete = model.PercentComplete,
            RequiresInspection = model.RequiresInspection
        };
    }

    private static TaskEditViewModel ToViewModel(TaskEntity task)
    {
        return new TaskEditViewModel
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            WorkItemId = task.WorkItemId,
            ParentTaskId = task.ParentTaskId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            PlannedStartDate = task.PlannedStartDate,
            PlannedEndDate = task.PlannedEndDate,
            ActualStartDate = task.ActualStartDate,
            ActualEndDate = task.ActualEndDate,
            Status = task.Status,
            PercentComplete = task.PercentComplete,
            RequiresInspection = task.RequiresInspection,
            AssigneeIds = task.Assignments.Select(assignment => assignment.UserId).ToList()
        };
    }
}
