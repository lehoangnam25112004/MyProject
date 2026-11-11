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

[Authorize(Roles = "Admin,Project Director,Project Manager,Site Manager,Supervising Engineer")]
public class ConstructionLogsController(
    ApplicationDbContext dbContext,
    IConstructionLogService logService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(int? projectId, CancellationToken cancellationToken)
    {
        var query = dbContext.ConstructionLogs.AsNoTracking()
            .Include(log => log.Project).Include(log => log.CreatedBy)
            .OrderByDescending(log => log.LogDate).AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(log => log.ProjectId == projectId.Value)
                .OrderByDescending(log => log.LogDate);
        }
        return View(new ConstructionLogIndexViewModel
        {
            Logs = await query.Take(100).ToListAsync(cancellationToken),
            Projects = await dbContext.Projects.Where(project => !project.IsArchived)
                .OrderBy(project => project.ProjectCode).ToListAsync(cancellationToken)
        });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);
        return View(new ConstructionLogEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ConstructionLogEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await logService.SaveAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã lưu nhật ký thi công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var log = await dbContext.ConstructionLogs.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (log is null) return NotFound();
        await LoadProjectsAsync(cancellationToken);
        return View(new ConstructionLogEditViewModel
        {
            Id = log.Id, ProjectId = log.ProjectId, LogDate = log.LogDate,
            Weather = log.Weather ?? string.Empty, Workforce = log.Workforce,
            WorkPerformed = log.WorkPerformed ?? string.Empty,
            MaterialsUsed = log.MaterialsUsed, EquipmentUsed = log.EquipmentUsed,
            Issues = log.Issues
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, ConstructionLogEditViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await logService.SaveAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã cập nhật nhật ký.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadProjectsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Projects = await dbContext.Projects.Where(project => !project.IsArchived)
            .OrderBy(project => project.ProjectCode).ToListAsync(cancellationToken);
    }
}
