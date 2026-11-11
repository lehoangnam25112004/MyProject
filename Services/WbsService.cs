using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class WbsService(ApplicationDbContext dbContext) : IWbsService
{
    public Task<Project?> GetProjectTreeAsync(
        int projectId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Phases.OrderBy(phase => phase.SequenceOrder))
                .ThenInclude(phase => phase.WorkItems.OrderBy(item => item.SequenceOrder))
            .SingleOrDefaultAsync(project => project.Id == projectId && !project.IsArchived, cancellationToken);

        return query;
    }

    public async Task<(bool Succeeded, string? Error, ProjectPhase? Phase)> CreatePhaseAsync(
        ProjectPhase phase,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        if (!await CanAccessProjectAsync(phase.ProjectId, userId, isAdmin, cancellationToken))
        {
            return (false, "Bạn không có quyền trên dự án này.", null);
        }

        if (string.IsNullOrWhiteSpace(phase.Name))
        {
            return (false, "Tên giai đoạn không được để trống.", null);
        }

        phase.Name = phase.Name.Trim();
        dbContext.ProjectPhases.Add(phase);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, phase);
    }

    public async Task<(bool Succeeded, string? Error, WorkItem? WorkItem)> CreateWorkItemAsync(
        WorkItem workItem,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        if (!await CanAccessProjectAsync(workItem.ProjectId, userId, isAdmin, cancellationToken))
        {
            return (false, "Bạn không có quyền trên dự án này.", null);
        }

        var phase = await dbContext.ProjectPhases
            .SingleOrDefaultAsync(item => item.Id == workItem.ProjectPhaseId, cancellationToken);
        if (phase is null || phase.ProjectId != workItem.ProjectId)
        {
            return (false, "Giai đoạn không thuộc dự án đã chọn.", null);
        }

        if (workItem.ParentWorkItemId.HasValue)
        {
            var parent = await dbContext.WorkItems
                .SingleOrDefaultAsync(item => item.Id == workItem.ParentWorkItemId, cancellationToken);
            if (parent is null || parent.ProjectId != workItem.ProjectId)
            {
                return (false, "Công việc cha phải thuộc cùng dự án.", null);
            }

            workItem.Level = parent.Level + 1;
        }
        else
        {
            workItem.Level = 1;
        }

        var duplicateCode = await dbContext.WorkItems.AnyAsync(
            item => item.WbsCode == workItem.WbsCode.Trim() && item.ProjectId == workItem.ProjectId,
            cancellationToken);
        if (duplicateCode)
        {
            return (false, "Mã WBS đã tồn tại trong dự án.", null);
        }

        workItem.WbsCode = workItem.WbsCode.Trim();
        workItem.Name = workItem.Name.Trim();
        dbContext.WorkItems.Add(workItem);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, workItem);
    }

    private async Task<bool> CanAccessProjectAsync(
        int projectId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        if (isAdmin)
        {
            return await dbContext.Projects.AnyAsync(
                project => project.Id == projectId && !project.IsArchived,
                cancellationToken);
        }

        return await dbContext.Projects.AnyAsync(
            project => project.Id == projectId &&
                       !project.IsArchived &&
                       (project.ProjectManagerId == userId ||
                        project.Members.Any(member => member.UserId == userId && member.IsActive)),
            cancellationToken);
    }
}
