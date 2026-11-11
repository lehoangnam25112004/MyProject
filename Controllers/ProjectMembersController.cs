using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Controllers;

[Authorize(Roles = "Admin,Project Director,Project Manager")]
public class ProjectMembersController(
    IEmployeeService employeeService,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int projectId, CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
            .Include(item => item.Members)
                .ThenInclude(member => member.User)
            .SingleOrDefaultAsync(item => item.Id == projectId && !item.IsArchived, cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        ViewBag.Project = project;
        ViewBag.Users = await userManager.Users
            .Where(user => user.IsActive && !project.Members.Select(member => member.UserId).Contains(user.Id))
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);
        return View(new ProjectMemberViewModel { ProjectId = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        ProjectMemberViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
        }

        var result = await employeeService.AddProjectMemberAsync(
            model.ProjectId,
            model.UserId,
            model.AssignmentRole,
            cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã thêm thành viên dự án." : result.Error;
        return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
    }
}
