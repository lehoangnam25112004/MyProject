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

[Authorize(Roles = "Admin,Project Director")]
public class EmployeesController(
    IEmployeeService employeeService,
    ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await employeeService.GetEmployeesAsync(cancellationToken);
        return View(new EmployeeListViewModel { Employees = employees });
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadOptionsAsync(cancellationToken);
        return View(new EmployeeEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EmployeeEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        var result = await employeeService.SaveAsync(ToEntity(model), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo hồ sơ nhân viên.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        await LoadOptionsAsync(cancellationToken);
        return View(ToViewModel(employee));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EmployeeEditViewModel model,
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

        var result = await employeeService.SaveAsync(ToEntity(model, id), cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadOptionsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật hồ sơ nhân viên.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        ViewBag.Users = await dbContext.Users
            .Where(user => user.IsActive)
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);
        ViewBag.Departments = await employeeService.GetDepartmentsAsync(cancellationToken);
    }

    private static Employee ToEntity(EmployeeEditViewModel model, int? id = null)
    {
        return new Employee
        {
            Id = id ?? 0,
            EmployeeCode = model.EmployeeCode,
            UserId = model.UserId,
            DepartmentId = model.DepartmentId,
            Position = model.Position,
            Phone = model.Phone,
            HireDate = model.HireDate,
            IsActive = model.IsActive
        };
    }

    private static EmployeeEditViewModel ToViewModel(Employee employee)
    {
        return new EmployeeEditViewModel
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            UserId = employee.UserId,
            DepartmentId = employee.DepartmentId,
            Position = employee.Position,
            Phone = employee.Phone,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive
        };
    }
}
