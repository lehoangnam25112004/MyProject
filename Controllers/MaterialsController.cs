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

[Authorize(Roles = "Admin,Project Director,Project Manager,Site Manager,Materials Staff")]
public class MaterialsController(
    ApplicationDbContext dbContext,
    IMaterialService materialService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new MaterialIndexViewModel
        {
            Materials = await dbContext.Materials
                .AsNoTracking()
                .OrderBy(material => material.MaterialCode)
                .ToListAsync(cancellationToken),
            Requests = await dbContext.MaterialRequests
                .AsNoTracking()
                .Include(request => request.Project)
                .Include(request => request.RequestedBy)
                .Include(request => request.Items)
                    .ThenInclude(item => item.Material)
                .OrderByDescending(request => request.RequestedAt)
                .Take(100)
                .ToListAsync(cancellationToken)
        };
        return View(model);
    }

    public IActionResult Create()
    {
        return View(new MaterialEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        MaterialEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await materialService.SaveMaterialAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã lưu vật tư.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var material = await dbContext.Materials.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return material is null
            ? NotFound()
            : View(new MaterialEditViewModel
            {
                Id = material.Id,
                MaterialCode = material.MaterialCode,
                Name = material.Name,
                Unit = material.Unit,
                CurrentStock = material.CurrentStock,
                IsActive = material.IsActive
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        MaterialEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await materialService.SaveMaterialAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật vật tư.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> CreateRequest(CancellationToken cancellationToken)
    {
        await LoadRequestOptionsAsync(cancellationToken);
        return View(new MaterialRequestCreateViewModel
        {
            Items = [new MaterialRequestLineViewModel()]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(
        MaterialRequestCreateViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadRequestOptionsAsync(cancellationToken);
            return View(model);
        }

        var userId = userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var result = await materialService.CreateRequestAsync(model, userId, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadRequestOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo phiếu yêu cầu vật tư.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(
        int id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatusAsync(id, MaterialRequestStatus.Approved, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(
        int id,
        CancellationToken cancellationToken)
    {
        return await ChangeStatusAsync(id, MaterialRequestStatus.Rejected, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var result = await materialService.IssueRequestAsync(id, userId, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã xuất vật tư và cập nhật tồn kho." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ChangeStatusAsync(
        int id,
        MaterialRequestStatus status,
        CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var result = await materialService.SetRequestStatusAsync(
            id, status, userId, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded ? "Đã cập nhật trạng thái phiếu." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadRequestOptionsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Projects = await dbContext.Projects
            .Where(project => !project.IsArchived)
            .OrderBy(project => project.ProjectCode)
            .ToListAsync(cancellationToken);
        ViewBag.Tasks = await dbContext.Tasks
            .Where(task => task.Status != TaskStatus.Cancelled)
            .OrderBy(task => task.Title)
            .ToListAsync(cancellationToken);
        ViewBag.Materials = await dbContext.Materials
            .Where(material => material.IsActive)
            .OrderBy(material => material.MaterialCode)
            .ToListAsync(cancellationToken);
    }
}
