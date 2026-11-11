using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ConstructionProjectManagement.Models.TaskStatus;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Controllers;

[Authorize(Roles = "Admin,Project Director,Project Manager")]
public class CompletionController(
    ApplicationDbContext dbContext,
    ICompletionService completionService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(new CompletionIndexViewModel
        {
            Projects = await dbContext.Projects.Where(p => !p.IsArchived).OrderBy(p => p.ProjectCode).ToListAsync(cancellationToken),
            Expenses = await dbContext.ProjectExpenses.AsNoTracking().Include(e => e.Project).OrderByDescending(e => e.ExpenseDate).Take(100).ToListAsync(cancellationToken),
            Handovers = await dbContext.ProjectHandovers.AsNoTracking().Include(h => h.Project).OrderByDescending(h => h.HandoverDate).Take(100).ToListAsync(cancellationToken),
            OpenTasks = await dbContext.Tasks.AsNoTracking().Include(t => t.Project).Where(t => t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled).OrderBy(t => t.PlannedEndDate).Take(100).ToListAsync(cancellationToken),
            TotalExpense = await dbContext.ProjectExpenses.SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0
        });
    }

    public async Task<IActionResult> CreateExpense(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);
        return View(new ExpenseCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExpense(ExpenseCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { await LoadProjectsAsync(cancellationToken); return View(model); }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await completionService.AddExpenseAsync(model, userId, cancellationToken);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); await LoadProjectsAsync(cancellationToken); return View(model); }
        TempData["SuccessMessage"] = "Đã ghi nhận khoản chi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> CreateHandover(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);
        return View(new HandoverCreateViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHandover(HandoverCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) { await LoadProjectsAsync(cancellationToken); return View(model); }
        var userId = userManager.GetUserId(User);
        if (userId is null) return Challenge();
        var result = await completionService.CreateHandoverAsync(model, userId, cancellationToken);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); await LoadProjectsAsync(cancellationToken); return View(model); }
        TempData["SuccessMessage"] = "Đã lập hồ sơ bàn giao.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmHandover(int id, CancellationToken cancellationToken)
    {
        var result = await completionService.ConfirmHandoverAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded ? "Đã xác nhận bàn giao." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id, CancellationToken cancellationToken)
    {
        var result = await completionService.ArchiveProjectAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded ? "Đã đóng và lưu trữ dự án." : result.Error!;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadProjectsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Projects = await dbContext.Projects.Where(p => !p.IsArchived).OrderBy(p => p.ProjectCode).ToListAsync(cancellationToken);
    }
}
