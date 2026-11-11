using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace ConstructionProjectManagement.Controllers;

[Authorize]
public class ContractsController(
    IContractService contractService,
    IProjectDocumentService documentService,
    IProjectService projectService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var contracts = await contractService.GetVisibleAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return View(new ContractListViewModel { Contracts = contracts });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var contract = await contractService.GetByIdAsync(
            id,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        return contract is null
            ? NotFound()
            : View(new ContractDetailsViewModel { Contract = contract });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadProjectsAsync(cancellationToken);
        return View(new ContractEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Create(
        ContractEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await contractService.CreateAsync(
            ToEntity(model),
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã tạo hợp đồng.";
        return RedirectToAction(nameof(Details), new { id = result.Contract!.Id });
    }

    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var contract = await contractService.GetByIdAsync(
            id,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (contract is null)
        {
            return NotFound();
        }

        await LoadProjectsAsync(cancellationToken);
        return View(ToViewModel(contract));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> Edit(
        int id,
        ContractEditViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await contractService.UpdateAsync(
            ToEntity(model, id),
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await LoadProjectsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật hợp đồng.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Project Director,Project Manager")]
    public async Task<IActionResult> UploadDocument(
        int contractId,
        string category,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var contract = await contractService.GetByIdAsync(
            contractId,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (contract is null)
        {
            return NotFound();
        }

        var result = await documentService.UploadAsync(
            contract.ProjectId,
            contract.Id,
            category,
            file,
            user.Id,
            cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "Đã tải tài liệu lên.";
        }

        return RedirectToAction(nameof(Details), new { id = contractId });
    }

    public async Task<IActionResult> DownloadDocument(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var result = await documentService.GetDownloadAsync(
            id,
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
        if (result.Document is null || result.FilePath is null)
        {
            return NotFound();
        }

        return PhysicalFile(result.FilePath, result.Document.ContentType, result.Document.FileName);
    }

    private async Task LoadProjectsAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        ViewBag.Projects = await projectService.GetVisibleProjectsAsync(
            user.Id,
            User.IsInRole("Admin") || User.IsInRole("Project Director"),
            cancellationToken);
    }

    private static Contract ToEntity(ContractEditViewModel model, int? id = null)
    {
        return new Contract
        {
            Id = id ?? 0,
            ProjectId = model.ProjectId,
            ContractNumber = model.ContractNumber,
            ClientName = model.ClientName,
            ContractorName = model.ContractorName,
            ContractValue = model.ContractValue,
            SignedDate = model.SignedDate,
            EffectiveDate = model.EffectiveDate,
            Status = model.Status
        };
    }

    private static ContractEditViewModel ToViewModel(Contract contract)
    {
        return new ContractEditViewModel
        {
            Id = contract.Id,
            ProjectId = contract.ProjectId,
            ContractNumber = contract.ContractNumber,
            ClientName = contract.ClientName,
            ContractorName = contract.ContractorName,
            ContractValue = contract.ContractValue,
            SignedDate = contract.SignedDate,
            EffectiveDate = contract.EffectiveDate,
            Status = contract.Status
        };
    }
}
