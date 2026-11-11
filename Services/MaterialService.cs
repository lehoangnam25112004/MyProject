using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class MaterialService(ApplicationDbContext dbContext) : IMaterialService
{
    public async Task<(bool Succeeded, string? Error)> SaveMaterialAsync(
        MaterialEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        var code = model.MaterialCode.Trim();
        var duplicate = await dbContext.Materials.AnyAsync(
            material => material.MaterialCode == code && material.Id != model.Id,
            cancellationToken);
        if (duplicate)
        {
            return (false, "Mã vật tư đã tồn tại.");
        }

        if (model.Id == 0)
        {
            dbContext.Materials.Add(new Material
            {
                MaterialCode = code,
                Name = model.Name.Trim(),
                Unit = model.Unit.Trim(),
                CurrentStock = model.CurrentStock,
                IsActive = model.IsActive
            });
        }
        else
        {
            var material = await dbContext.Materials.SingleOrDefaultAsync(
                item => item.Id == model.Id, cancellationToken);
            if (material is null)
            {
                return (false, "Không tìm thấy vật tư.");
            }

            material.MaterialCode = code;
            material.Name = model.Name.Trim();
            material.Unit = model.Unit.Trim();
            material.CurrentStock = model.CurrentStock;
            material.IsActive = model.IsActive;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> CreateRequestAsync(
        MaterialRequestCreateViewModel model,
        string requestedById,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await dbContext.Projects.AnyAsync(
            project => project.Id == model.ProjectId && !project.IsArchived,
            cancellationToken);
        if (!projectExists)
        {
            return (false, "Dự án không hợp lệ.");
        }

        var lines = model.Items
            .Where(item => item.QuantityRequested > 0)
            .GroupBy(item => item.MaterialId)
            .Select(group => new { MaterialId = group.Key, Quantity = group.Sum(item => item.QuantityRequested) })
            .ToList();
        var materialIds = lines.Select(line => line.MaterialId).ToList();
        var activeMaterialIds = await dbContext.Materials
            .Where(material => material.IsActive && materialIds.Contains(material.Id))
            .Select(material => material.Id)
            .ToListAsync(cancellationToken);
        if (lines.Count == 0 || activeMaterialIds.Count != materialIds.Distinct().Count())
        {
            return (false, "Danh sách vật tư không hợp lệ.");
        }

        var request = new MaterialRequest
        {
            ProjectId = model.ProjectId,
            TaskId = model.TaskId,
            RequestedById = requestedById,
            Notes = model.Notes?.Trim(),
            Items = lines.Select(line => new MaterialRequestItem
            {
                MaterialId = line.MaterialId,
                QuantityRequested = line.Quantity
            }).ToList()
        };
        dbContext.MaterialRequests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> SetRequestStatusAsync(
        int requestId,
        MaterialRequestStatus status,
        string approvedById,
        CancellationToken cancellationToken = default)
    {
        if (status is not (MaterialRequestStatus.Approved or MaterialRequestStatus.Rejected))
        {
            return (false, "Trạng thái xử lý không hợp lệ.");
        }

        var request = await dbContext.MaterialRequests.SingleOrDefaultAsync(
            item => item.Id == requestId, cancellationToken);
        if (request is null || request.Status != MaterialRequestStatus.Pending)
        {
            return (false, "Phiếu không tồn tại hoặc đã được xử lý.");
        }

        request.Status = status;
        request.ApprovedById = approvedById;
        request.ApprovedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> IssueRequestAsync(
        int requestId,
        string performedById,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var request = await dbContext.MaterialRequests
            .Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.Id == requestId, cancellationToken);
        if (request is null || request.Status != MaterialRequestStatus.Approved)
        {
            return (false, "Chỉ phiếu đã duyệt mới được xuất vật tư.");
        }

        var materialIds = request.Items.Select(item => item.MaterialId).ToList();
        var materials = await dbContext.Materials
            .Where(material => materialIds.Contains(material.Id))
            .ToDictionaryAsync(material => material.Id, cancellationToken);
        foreach (var item in request.Items)
        {
            if (!materials.TryGetValue(item.MaterialId, out var material)
                || material.CurrentStock < item.QuantityRequested)
            {
                return (false, "Tồn kho không đủ để xuất toàn bộ phiếu.");
            }
        }

        foreach (var item in request.Items)
        {
            var material = materials[item.MaterialId];
            material.CurrentStock -= item.QuantityRequested;
            item.QuantityIssued = item.QuantityRequested;
            dbContext.MaterialTransactions.Add(new MaterialTransaction
            {
                MaterialId = material.Id,
                MaterialRequestId = request.Id,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId,
                Quantity = item.QuantityRequested,
                Type = MaterialTransactionType.Issue,
                PerformedById = performedById
            });
        }

        request.Status = MaterialRequestStatus.Issued;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return (true, null);
    }
}
