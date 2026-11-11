using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class EmployeeService(ApplicationDbContext dbContext) : IEmployeeService
{
    public async Task<IReadOnlyCollection<Employee>> GetEmployeesAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.User)
            .Include(employee => employee.Department)
            .OrderBy(employee => employee.EmployeeCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Succeeded, string? Error)> SaveAsync(
        Employee employee,
        CancellationToken cancellationToken = default)
    {
        var duplicate = await dbContext.Employees.AnyAsync(
            item => item.EmployeeCode == employee.EmployeeCode.Trim() && item.Id != employee.Id,
            cancellationToken);
        if (duplicate)
        {
            return (false, "Mã nhân viên đã tồn tại.");
        }

        var user = await dbContext.Users.SingleOrDefaultAsync(
            item => item.Id == employee.UserId && item.IsActive,
            cancellationToken);
        if (user is null)
        {
            return (false, "Chỉ tài khoản đang hoạt động được lập hồ sơ nhân viên.");
        }

        employee.EmployeeCode = employee.EmployeeCode.Trim();
        employee.Position = employee.Position?.Trim();
        employee.Phone = employee.Phone?.Trim();

        if (employee.Id == 0)
        {
            dbContext.Employees.Add(employee);
        }
        else
        {
            var existing = await dbContext.Employees.SingleOrDefaultAsync(
                item => item.Id == employee.Id,
                cancellationToken);
            if (existing is null)
            {
                return (false, "Không tìm thấy nhân viên.");
            }

            existing.EmployeeCode = employee.EmployeeCode;
            existing.UserId = employee.UserId;
            existing.DepartmentId = employee.DepartmentId;
            existing.Position = employee.Position;
            existing.Phone = employee.Phone;
            existing.HireDate = employee.HireDate;
            existing.IsActive = employee.IsActive;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<IReadOnlyCollection<Department>> GetDepartmentsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Succeeded, string? Error)> AddProjectMemberAsync(
        int projectId,
        string userId,
        string? assignmentRole,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await dbContext.Projects.AnyAsync(
            project => project.Id == projectId && !project.IsArchived,
            cancellationToken);
        var userExists = await dbContext.Users.AnyAsync(
            user => user.Id == userId && user.IsActive,
            cancellationToken);
        if (!projectExists || !userExists)
        {
            return (false, "Dự án hoặc tài khoản không hợp lệ.");
        }

        var duplicate = await dbContext.ProjectMembers.AnyAsync(
            member => member.ProjectId == projectId && member.UserId == userId,
            cancellationToken);
        if (duplicate)
        {
            return (false, "Nhân sự đã thuộc dự án.");
        }

        dbContext.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            AssignmentRole = assignmentRole?.Trim(),
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }
}
