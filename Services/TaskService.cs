using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskEntity = ConstructionProjectManagement.Models.Task;
using TaskStatusModel = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Services;

public sealed class TaskService(ApplicationDbContext dbContext) : ITaskService
{
    public async System.Threading.Tasks.Task<IReadOnlyCollection<TaskEntity>> GetVisibleAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        return await VisibleQuery(userId, isAdmin)
            .OrderBy(task => task.PlannedEndDate)
            .ToListAsync(cancellationToken);
    }

    public System.Threading.Tasks.Task<TaskEntity?> GetByIdAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        return VisibleQuery(userId, isAdmin)
            .SingleOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task<(bool Succeeded, string? Error, TaskEntity? Task)> CreateAsync(
        TaskEntity task,
        IEnumerable<string> assigneeIds,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateAsync(task, assigneeIds, userId, isAdmin, cancellationToken);
        if (validationError is not null)
        {
            return (false, validationError, null);
        }

        task.Title = task.Title.Trim();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        task.Assignments = BuildAssignments(assigneeIds);
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, task);
    }

    public async System.Threading.Tasks.Task<(bool Succeeded, string? Error)> UpdateAsync(
        TaskEntity task,
        IEnumerable<string> assigneeIds,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Tasks
            .Include(item => item.Assignments)
            .SingleOrDefaultAsync(item => item.Id == task.Id, cancellationToken);
        if (existing is null)
        {
            return (false, "Không tìm thấy công việc.");
        }

        var validationError = await ValidateAsync(task, assigneeIds, userId, isAdmin, cancellationToken);
        if (validationError is not null)
        {
            return (false, validationError);
        }

        existing.ProjectId = task.ProjectId;
        existing.WorkItemId = task.WorkItemId;
        existing.ParentTaskId = task.ParentTaskId;
        existing.Title = task.Title.Trim();
        existing.Description = task.Description?.Trim();
        existing.Priority = task.Priority;
        existing.PlannedStartDate = task.PlannedStartDate;
        existing.PlannedEndDate = task.PlannedEndDate;
        existing.ActualStartDate = task.ActualStartDate;
        existing.ActualEndDate = task.ActualEndDate;
        existing.Status = task.Status;
        existing.PercentComplete = task.PercentComplete;
        existing.RequiresInspection = task.RequiresInspection;
        existing.UpdatedAt = DateTime.UtcNow;

        dbContext.TaskAssignments.RemoveRange(existing.Assignments);
        existing.Assignments = BuildAssignments(assigneeIds);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private IQueryable<TaskEntity> VisibleQuery(string userId, bool isAdmin)
    {
        var query = dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.Project)
            .Include(task => task.WorkItem)
            .Include(task => task.Assignments)
                .ThenInclude(assignment => assignment.User)
            .Where(task => !task.Project.IsArchived);

        if (!isAdmin)
        {
            query = query.Where(task =>
                task.Project.ProjectManagerId == userId ||
                task.Project.Members.Any(member => member.UserId == userId && member.IsActive) ||
                task.Assignments.Any(assignment => assignment.UserId == userId));
        }

        return query;
    }

    private async Task<string?> ValidateAsync(
        TaskEntity task,
        IEnumerable<string> assigneeIds,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        if (task.PlannedEndDate < task.PlannedStartDate)
        {
            return "Ngày kết thúc không được trước ngày bắt đầu.";
        }

        if (task.PercentComplete is < 0 or > 100)
        {
            return "Phần trăm hoàn thành phải từ 0 đến 100.";
        }

        if (task.RequiresInspection && task.Status == TaskStatusModel.Completed)
        {
            return "Công việc cần nghiệm thu phải chuyển sang PendingInspection trước khi hoàn thành.";
        }

        var project = await dbContext.Projects
            .Include(item => item.Members)
            .SingleOrDefaultAsync(item => item.Id == task.ProjectId && !item.IsArchived, cancellationToken);
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

        if (task.WorkItemId.HasValue)
        {
            var workItem = await dbContext.WorkItems
                .SingleOrDefaultAsync(item => item.Id == task.WorkItemId, cancellationToken);
            if (workItem is null || workItem.ProjectId != task.ProjectId)
            {
                return "WBS phải thuộc cùng dự án.";
            }
        }

        if (task.ParentTaskId.HasValue)
        {
            var parent = await dbContext.Tasks
                .SingleOrDefaultAsync(item => item.Id == task.ParentTaskId, cancellationToken);
            if (parent is null || parent.ProjectId != task.ProjectId)
            {
                return "Công việc cha phải thuộc cùng dự án.";
            }
        }

        var ids = assigneeIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        var eligibleIds = await dbContext.Users
            .Where(user => ids.Contains(user.Id) &&
                           user.IsActive &&
                           (user.Id == project.ProjectManagerId ||
                            project.Members.Any(member => member.UserId == user.Id && member.IsActive)))
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);
        return eligibleIds.Count == ids.Count
            ? null
            : "Chỉ được phân công nhân sự đang hoạt động và thuộc dự án.";
    }

    private static List<TaskAssignment> BuildAssignments(IEnumerable<string> assigneeIds)
    {
        return assigneeIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .Select(id => new TaskAssignment { UserId = id })
            .ToList();
    }
}
