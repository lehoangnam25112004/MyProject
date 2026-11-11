using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskEntity = ConstructionProjectManagement.Models.Task;
using TaskStatusModel = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Services;

public sealed class ProgressService(ApplicationDbContext dbContext) : IProgressService
{
    public async Task<IReadOnlyCollection<TaskEntity>> GetVisibleTasksAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = VisibleQuery(userId, isAdmin);
        return await query
            .OrderBy(task => task.PlannedEndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Succeeded, string? Error)> UpdateAsync(
        int taskId,
        decimal percentComplete,
        TaskStatusModel status,
        string? delayReason,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        if (percentComplete is < 0 or > 100)
        {
            return (false, "Phần trăm hoàn thành phải từ 0 đến 100.");
        }

        var task = await VisibleQuery(userId, isAdmin)
            .SingleOrDefaultAsync(item => item.Id == taskId, cancellationToken);
        if (task is null)
        {
            return (false, "Không tìm thấy công việc hoặc bạn không có quyền truy cập.");
        }

        if (task.RequiresInspection && status == TaskStatusModel.Completed)
        {
            return (false, "Công việc cần nghiệm thu chưa thể chuyển Completed.");
        }

        var isOverdue = task.PlannedEndDate.Date < DateTime.UtcNow.Date &&
                        percentComplete < 100;
        if (isOverdue && string.IsNullOrWhiteSpace(delayReason))
        {
            return (false, "Công việc chậm tiến độ phải có nguyên nhân.");
        }

        dbContext.TaskProgressHistories.Add(new TaskProgressHistory
        {
            TaskId = task.Id,
            PreviousPercentComplete = task.PercentComplete,
            NewPercentComplete = percentComplete,
            PreviousStatus = task.Status,
            NewStatus = status,
            DelayReason = delayReason?.Trim(),
            UpdatedById = userId,
            UpdatedAt = DateTime.UtcNow
        });

        task.PercentComplete = percentComplete;
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        if (percentComplete > 0 && !task.ActualStartDate.HasValue)
        {
            task.ActualStartDate = DateTime.UtcNow;
        }

        if (percentComplete == 100)
        {
            task.ActualEndDate = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyCollection<ProgressChartPoint>> GetChartAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var tasks = await VisibleQuery(userId, isAdmin)
            .ToListAsync(cancellationToken);

        return tasks
            .GroupBy(task => task.Project.Name)
            .Select(group => new ProgressChartPoint(
                group.Key,
                group.Average(task => CalculatePlannedPercent(task, DateTime.UtcNow.Date)),
                group.Average(task => task.PercentComplete)))
            .ToList();
    }

    private IQueryable<TaskEntity> VisibleQuery(string userId, bool isAdmin)
    {
        var query = dbContext.Tasks
            .Include(task => task.Project)
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

    private static decimal CalculatePlannedPercent(TaskEntity task, DateTime today)
    {
        if (today <= task.PlannedStartDate.Date)
        {
            return 0;
        }

        if (today >= task.PlannedEndDate.Date)
        {
            return 100;
        }

        var totalDays = (task.PlannedEndDate.Date - task.PlannedStartDate.Date).TotalDays;
        var elapsedDays = (today - task.PlannedStartDate.Date).TotalDays;
        return totalDays <= 0 ? 100 : (decimal)(elapsedDays / totalDays * 100);
    }
}
