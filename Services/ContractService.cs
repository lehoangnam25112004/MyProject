using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class ContractService(ApplicationDbContext dbContext) : IContractService
{
    public async Task<IReadOnlyCollection<Contract>> GetVisibleAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = VisibleQuery(userId, isAdmin);
        return await query.OrderByDescending(contract => contract.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Contract?> GetByIdAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        return VisibleQuery(userId, isAdmin)
            .SingleOrDefaultAsync(contract => contract.Id == id, cancellationToken);
    }

    public async Task<(bool Succeeded, string? Error, Contract? Contract)> CreateAsync(
        Contract contract,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var error = await ValidateAsync(contract, userId, isAdmin, cancellationToken);
        if (error is not null)
        {
            return (false, error, null);
        }

        contract.ContractNumber = contract.ContractNumber.Trim();
        contract.ClientName = contract.ClientName?.Trim();
        contract.ContractorName = contract.ContractorName?.Trim();
        contract.CreatedAt = DateTime.UtcNow;
        contract.UpdatedAt = DateTime.UtcNow;
        dbContext.Contracts.Add(contract);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, contract);
    }

    public async Task<(bool Succeeded, string? Error)> UpdateAsync(
        Contract contract,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Contracts
            .SingleOrDefaultAsync(item => item.Id == contract.Id && !item.IsArchived, cancellationToken);
        if (existing is null)
        {
            return (false, "Không tìm thấy hợp đồng.");
        }

        var error = await ValidateAsync(contract, userId, isAdmin, cancellationToken, contract.Id);
        if (error is not null)
        {
            return (false, error);
        }

        existing.ProjectId = contract.ProjectId;
        existing.ContractNumber = contract.ContractNumber.Trim();
        existing.ClientName = contract.ClientName?.Trim();
        existing.ContractorName = contract.ContractorName?.Trim();
        existing.ContractValue = contract.ContractValue;
        existing.SignedDate = contract.SignedDate;
        existing.EffectiveDate = contract.EffectiveDate;
        existing.Status = contract.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private IQueryable<Contract> VisibleQuery(string userId, bool isAdmin)
    {
        var query = dbContext.Contracts
            .AsNoTracking()
            .Include(contract => contract.Project)
            .Include(contract => contract.Documents)
            .Where(contract => !contract.IsArchived && !contract.Project.IsArchived);

        if (!isAdmin)
        {
            query = query.Where(contract =>
                contract.Project.ProjectManagerId == userId ||
                contract.Project.Members.Any(member => member.UserId == userId && member.IsActive));
        }

        return query;
    }

    private async Task<string?> ValidateAsync(
        Contract contract,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken,
        int? currentId = null)
    {
        if (contract.ContractValue < 0)
        {
            return "Giá trị hợp đồng không được âm.";
        }

        if (contract.SignedDate.HasValue &&
            contract.EffectiveDate.HasValue &&
            contract.EffectiveDate < contract.SignedDate)
        {
            return "Ngày hiệu lực không được trước ngày ký.";
        }

        var project = await dbContext.Projects
            .Include(item => item.Members)
            .SingleOrDefaultAsync(item => item.Id == contract.ProjectId && !item.IsArchived, cancellationToken);
        if (project is null)
        {
            return "Dự án không hợp lệ.";
        }

        if (!isAdmin &&
            project.ProjectManagerId != userId &&
            !project.Members.Any(member => member.UserId == userId && member.IsActive))
        {
            return "Bạn không có quyền trên dự án này.";
        }

        var duplicate = await dbContext.Contracts.AnyAsync(
            item => item.ContractNumber == contract.ContractNumber.Trim() &&
                    !item.IsArchived &&
                    item.Id != currentId,
            cancellationToken);
        return duplicate ? "Số hợp đồng đã tồn tại." : null;
    }
}
