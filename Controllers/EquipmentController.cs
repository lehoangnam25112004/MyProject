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

[Authorize(Roles = "Admin,Project Director,Site Manager")]
public class EquipmentController(
    ApplicationDbContext dbContext,
    IEquipmentService equipmentService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new EquipmentIndexViewModel
        {
            Equipment = await dbContext.Equipment.AsNoTracking()
                .OrderBy(item => item.EquipmentCode).ToListAsync(cancellationToken),
            Assignments = await dbContext.EquipmentAssignments.AsNoTracking()
                .Include(item => item.Equipment)
                .Include(item => item.Project)
                .Include(item => item.AssignedTo)
                .Where(item => !item.ReturnedAt.HasValue)
                .OrderByDescending(item => item.HandedOverAt)
                .ToListAsync(cancellationToken)
        };
        return View(model);
    }

    public IActionResult Create() => View(new EquipmentEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EquipmentEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await equipmentService.SaveAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã lưu thiết bị.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var item = await dbContext.Equipment.AsNoTracking()
            .SingleOrDefaultAsync(equipment => equipment.Id == id, cancellationToken);
        return item is null ? NotFound() : View(new EquipmentEditViewModel
        {
            Id = item.Id, EquipmentCode = item.EquipmentCode, Name = item.Name,
            Category = item.Category, Status = item.Status, Notes = item.Notes,
            IsActive = item.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, EquipmentEditViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await equipmentService.SaveAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã cập nhật thiết bị.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Assign(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        return View(new EquipmentAssignmentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(
        EquipmentAssignmentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }
        var userId = userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }
        var result = await equipmentService.AssignAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }
        TempData["SuccessMessage"] = "Đã bàn giao thiết bị.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id, CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }
        var result = await equipmentService.ReturnAsync(id, userId, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã thu hồi thiết bị." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetMaintenance(
        int id, CancellationToken cancellationToken)
    {
        var result = await equipmentService.SetStatusAsync(
            id, EquipmentStatus.Maintenance, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã chuyển thiết bị sang bảo trì." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Equipment = await dbContext.Equipment
            .Where(item => item.IsActive && item.Status == EquipmentStatus.Available)
            .OrderBy(item => item.EquipmentCode).ToListAsync(cancellationToken);
        ViewBag.Projects = await dbContext.Projects
            .Where(project => !project.IsArchived)
            .OrderBy(project => project.ProjectCode).ToListAsync(cancellationToken);
        ViewBag.Tasks = await dbContext.Tasks
            .Where(task => task.Status != TaskStatus.Cancelled)
            .OrderBy(task => task.Title).ToListAsync(cancellationToken);
        ViewBag.Users = await dbContext.Users
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName).ToListAsync(cancellationToken);
    }
}
