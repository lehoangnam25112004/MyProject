using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> SaveAsync(
        Employee employee,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string? Error)> AddProjectMemberAsync(
        int projectId,
        string userId,
        string? assignmentRole,
        CancellationToken cancellationToken = default);
}
