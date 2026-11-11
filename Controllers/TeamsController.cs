using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Controllers;

[Authorize(Roles = "Admin,Project Director")]
public class TeamsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var teams = await dbContext.ConstructionTeams
            .Include(team => team.Members)
                .ThenInclude(member => member.Employee)
                    .ThenInclude(employee => employee.User)
            .OrderBy(team => team.Name)
            .ToListAsync(cancellationToken);
        ViewBag.Employees = await dbContext.Employees
            .Include(employee => employee.User)
            .Where(employee => employee.IsActive && employee.User.IsActive)
            .OrderBy(employee => employee.EmployeeCode)
            .ToListAsync(cancellationToken);
        return View(teams);
    }

    public IActionResult Create()
    {
        return View(new TeamEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TeamEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        dbContext.ConstructionTeams.Add(new ConstructionTeam
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim()
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(
        TeamMemberAddViewModel model,
        CancellationToken cancellationToken)
    {
        var employeeExists = await dbContext.Employees.AnyAsync(
            employee => employee.Id == model.EmployeeId && employee.IsActive && employee.User.IsActive,
            cancellationToken);
        var duplicate = await dbContext.TeamMembers.AnyAsync(
            member => member.TeamId == model.TeamId && member.EmployeeId == model.EmployeeId,
            cancellationToken);
        if (employeeExists && !duplicate)
        {
            dbContext.TeamMembers.Add(new TeamMember
            {
                TeamId = model.TeamId,
                EmployeeId = model.EmployeeId
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }
}
