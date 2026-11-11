using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IWbsService
{
    Task<Project?> GetProjectTreeAsync(
        int projectId,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error, ProjectPhase? Phase)> CreatePhaseAsync(
        ProjectPhase phase,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error, WorkItem? WorkItem)> CreateWorkItemAsync(
        WorkItem workItem,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
