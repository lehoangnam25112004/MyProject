using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class EquipmentService(ApplicationDbContext dbContext) : IEquipmentService
{
    public async Task<(bool Succeeded, string? Error)> SaveAsync(
        EquipmentEditViewModel model,
        CancellationToken cancellationToken = default)
    {
        var code = model.EquipmentCode.Trim();
        if (await dbContext.Equipment.AnyAsync(
                item => item.EquipmentCode == code && item.Id != model.Id, cancellationToken))
        {
            return (false, "Mã thiết bị đã tồn tại.");
        }

        if (model.Id == 0)
        {
            dbContext.Equipment.Add(new Equipment
            {
                EquipmentCode = code,
                Name = model.Name.Trim(),
                Category = model.Category?.Trim(),
                Status = model.Status,
                Notes = model.Notes?.Trim(),
                IsActive = model.IsActive
            });
        }
        else
        {
            var equipment = await dbContext.Equipment.SingleOrDefaultAsync(
                item => item.Id == model.Id, cancellationToken);
            if (equipment is null)
            {
                return (false, "Không tìm thấy thiết bị.");
            }

            if (equipment.Status == EquipmentStatus.InUse && model.Status != EquipmentStatus.InUse)
            {
                return (false, "Thiết bị đang được bàn giao, hãy thu hồi trước khi đổi trạng thái.");
            }

            equipment.EquipmentCode = code;
            equipment.Name = model.Name.Trim();
            equipment.Category = model.Category?.Trim();
            equipment.Status = model.Status;
            equipment.Notes = model.Notes?.Trim();
            equipment.IsActive = model.IsActive;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> AssignAsync(
        EquipmentAssignmentViewModel model,
        string handedOverById,
        CancellationToken cancellationToken = default)
    {
        var equipment = await dbContext.Equipment.SingleOrDefaultAsync(
            item => item.Id == model.EquipmentId && item.IsActive, cancellationToken);
        if (equipment is null || equipment.Status != EquipmentStatus.Available)
        {
            return (false, "Thiết bị không tồn tại hoặc chưa sẵn sàng.");
        }

        var projectExists = await dbContext.Projects.AnyAsync(
            project => project.Id == model.ProjectId && !project.IsArchived, cancellationToken);
        var userExists = await dbContext.Users.AnyAsync(
            user => user.Id == model.AssignedToId && user.IsActive, cancellationToken);
        if (!projectExists || !userExists)
        {
            return (false, "Dự án hoặc người nhận không hợp lệ.");
        }

        equipment.Status = EquipmentStatus.InUse;
        dbContext.EquipmentAssignments.Add(new EquipmentAssignment
        {
            EquipmentId = model.EquipmentId,
            ProjectId = model.ProjectId,
            TaskId = model.TaskId,
            AssignedToId = model.AssignedToId,
            HandedOverById = handedOverById,
            Notes = model.Notes?.Trim()
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> ReturnAsync(
        int assignmentId,
        string returnedById,
        CancellationToken cancellationToken = default)
    {
        var assignment = await dbContext.EquipmentAssignments
            .Include(item => item.Equipment)
            .SingleOrDefaultAsync(item => item.Id == assignmentId, cancellationToken);
        if (assignment is null || assignment.ReturnedAt.HasValue)
        {
            return (false, "Lần bàn giao không tồn tại hoặc đã thu hồi.");
        }

        assignment.ReturnedAt = DateTime.UtcNow;
        assignment.ReturnedById = returnedById;
        assignment.Equipment.Status = EquipmentStatus.Available;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> SetStatusAsync(
        int equipmentId,
        EquipmentStatus status,
        CancellationToken cancellationToken = default)
    {
        var equipment = await dbContext.Equipment.SingleOrDefaultAsync(
            item => item.Id == equipmentId, cancellationToken);
        if (equipment is null)
        {
            return (false, "Không tìm thấy thiết bị.");
        }
        if (equipment.Status == EquipmentStatus.InUse && status != EquipmentStatus.InUse)
        {
            return (false, "Thiết bị đang được sử dụng, hãy thu hồi trước.");
        }
        equipment.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }
}
