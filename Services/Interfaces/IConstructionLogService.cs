using ConstructionProjectManagement.ViewModels;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IConstructionLogService
{
    Task<(bool Succeeded, string? Error)> SaveAsync(
        ConstructionLogEditViewModel model, string userId,
        CancellationToken cancellationToken = default);
}
