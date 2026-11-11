using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.ViewModels;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IEquipmentService
{
    Task<(bool Succeeded, string? Error)> SaveAsync(
        EquipmentEditViewModel model,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> AssignAsync(
        EquipmentAssignmentViewModel model,
        string handedOverById,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> ReturnAsync(
        int assignmentId,
        string returnedById,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> SetStatusAsync(
        int equipmentId,
        EquipmentStatus status,
        CancellationToken cancellationToken = default);
}
