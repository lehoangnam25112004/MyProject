using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var accounts = new List<AccountListItemViewModel>(users.Count);
        foreach (var user in users)
        {
            accounts.Add(new AccountListItemViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                IsActive = user.IsActive,
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            });
        }

        return View(new AccountListViewModel { Accounts = accounts });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await GetRoleNamesAsync();
        return View(new AccountEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(AccountEditViewModel model)
    {
        ViewBag.Roles = await GetRoleNamesAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Vui lòng nhập mật khẩu.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email?.Trim(),
            FullName = model.FullName.Trim(),
            IsActive = model.IsActive,
            LockoutEnabled = true,
            LockoutEnd = model.IsActive ? null : DateTimeOffset.MaxValue
        };

        var createResult = await userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                AddIdentityErrors(roleResult);
                return View(model);
            }
        }

        TempData["SuccessMessage"] = "Đã tạo tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        ViewBag.Roles = await GetRoleNamesAsync();
        return View(new AccountEditViewModel
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id, AccountEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ViewBag.Roles = await GetRoleNamesAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        user.FullName = model.FullName.Trim();
        user.Email = model.Email?.Trim();
        user.UserName = model.UserName.Trim();
        user.IsActive = model.IsActive;
        user.LockoutEnabled = true;
        user.LockoutEnd = model.IsActive ? null : DateTimeOffset.MaxValue;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            AddIdentityErrors(updateResult);
            return View(model);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                AddIdentityErrors(removeResult);
                return View(model);
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Role))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, model.Role);
            if (!addRoleResult.Succeeded)
            {
                AddIdentityErrors(addRoleResult);
                return View(model);
            }
        }

        TempData["SuccessMessage"] = "Đã cập nhật tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        return View(new ProfileViewModel
        {
            FullName = user.FullName ?? string.Empty,
            Email = user.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
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

        user.FullName = model.FullName.Trim();
        user.Email = model.Email?.Trim();
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật thông tin cá nhân.";
        return RedirectToAction(nameof(Profile));
    }

    private async Task<IReadOnlyCollection<string>> GetRoleNamesAsync()
    {
        return await roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
