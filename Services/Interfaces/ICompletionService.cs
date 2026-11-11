using ConstructionProjectManagement.ViewModels;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface ICompletionService
{
    Task<(bool Succeeded, string? Error)> AddExpenseAsync(ExpenseCreateViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> CreateHandoverAsync(HandoverCreateViewModel model, string userId, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> ConfirmHandoverAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, string? Error)> ArchiveProjectAsync(int projectId, CancellationToken cancellationToken = default);
}
