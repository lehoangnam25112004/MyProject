using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IContractService
{
    Task<IReadOnlyCollection<Contract>> GetVisibleAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Contract?> GetByIdAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error, Contract? Contract)> CreateAsync(
        Contract contract,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> UpdateAsync(
        Contract contract,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
