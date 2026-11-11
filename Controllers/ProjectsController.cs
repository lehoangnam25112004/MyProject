using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class ProjectsController(
    IProjectService projectService,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Challenge();
        }

        var projects = await projectService.GetVisibleProjectsAsync(
            currentUser.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);

        return View(new ProjectListViewModel { Projects = projects });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Challenge();
        }

        var project = await projectService.GetByIdAsync(
            id,
            currentUser.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);

        return project is null ? NotFound() : View(project);
    }

    [Authorize(Roles = "Admin,Project Director")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadUsersAsync(cancellationToken);
        return View(new ProjectEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director")]
    public async Task<IActionResult> Create(ProjectEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadUsersAsync(cancellationToken);
            return View(model);
        }

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Challenge();
        }

        var result = await projectService.CreateAsync(
            ToEntity(model),
            model.ProjectManagerId,
            model.MemberIds,
            currentUser.Id,
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadUsersAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo dự án.";
        return RedirectToAction(nameof(Details), new { id = result.Project!.Id });
    }

    [Authorize(Roles = "Admin,Project Director")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(item => item.Members)
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsArchived, cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        await LoadUsersAsync(cancellationToken);
        return View(ToViewModel(project));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director")]
    public async Task<IActionResult> Edit(
        int id,
        ProjectEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadUsersAsync(cancellationToken);
            return View(model);
        }

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Challenge();
        }

        var result = await projectService.UpdateAsync(
            ToEntity(model, id),
            model.ProjectManagerId,
            model.MemberIds,
            currentUser.Id,
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadUsersAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật dự án.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadUsersAsync(CancellationToken cancellationToken)
    {
        ViewBag.Users = await userManager.Users
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);
    }

    private static Project ToEntity(ProjectEditViewModel model, int? id = null)
    {
        return new Project
        {
            Id = id ?? 0,
            ProjectCode = model.ProjectCode,
            Name = model.Name,
            Location = model.Location,
            ClientName = model.ClientName,
            StartDate = model.StartDate,
            ExpectedEndDate = model.ExpectedEndDate,
            Budget = model.Budget,
            Status = model.Status
        };
    }

    private static ProjectEditViewModel ToViewModel(Project project)
    {
        return new ProjectEditViewModel
        {
            Id = project.Id,
            ProjectCode = project.ProjectCode,
            Name = project.Name,
            Location = project.Location,
            ClientName = project.ClientName,
            StartDate = project.StartDate,
            ExpectedEndDate = project.ExpectedEndDate,
            Budget = project.Budget,
            Status = project.Status,
            ProjectManagerId = project.ProjectManagerId,
            MemberIds = project.Members.Select(member => member.UserId).ToList()
        };
    }
}
