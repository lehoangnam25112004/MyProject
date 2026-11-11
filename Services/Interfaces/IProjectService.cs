using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyCollection<Project>> GetVisibleProjectsAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error, Project? Project)> CreateAsync(
        Project project,
        string? managerId,
        IEnumerable<string> memberIds,
        string currentUserId,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> UpdateAsync(
        Project project,
        string? managerId,
        IEnumerable<string> memberIds,
        string currentUserId,
        CancellationToken cancellationToken = default);
}
