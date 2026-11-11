using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.ViewModels;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IMaterialService
{
    Task<(bool Succeeded, string? Error)> SaveMaterialAsync(
        MaterialEditViewModel model,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> CreateRequestAsync(
        MaterialRequestCreateViewModel model,
        string requestedById,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> SetRequestStatusAsync(
        int requestId,
        MaterialRequestStatus status,
        string approvedById,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> IssueRequestAsync(
        int requestId,
        string performedById,
        CancellationToken cancellationToken = default);
}
