using ConstructionProjectManagement.Models;
using Task = System.Threading.Tasks.Task;
using TaskEntity = ConstructionProjectManagement.Models.Task;
using TaskStatusModel = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IProgressService
{
    Task<IReadOnlyCollection<TaskEntity>> GetVisibleTasksAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> UpdateAsync(
        int taskId,
        decimal percentComplete,
        TaskStatusModel status,
        string? delayReason,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProgressChartPoint>> GetChartAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}

public sealed record ProgressChartPoint(string Label, decimal Planned, decimal Actual);
