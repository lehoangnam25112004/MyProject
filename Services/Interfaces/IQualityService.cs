using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.ViewModels;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IQualityService
{
    Task<(bool Succeeded, string? Error)> CreateIncidentAsync(
        IncidentCreateViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> CreateRfiAsync(
        RfiCreateViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> RespondRfiAsync(
        int id, RfiRespondViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> CreateInspectionAsync(
        InspectionCreateViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> RecordInspectionAsync(
        int id, InspectionResultViewModel model, string userId, CancellationToken cancellationToken = default);
}
