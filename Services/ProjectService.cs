using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class ProjectService(ApplicationDbContext dbContext) : IProjectService
{
    public async Task<IReadOnlyCollection<Project>> GetVisibleProjectsAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = dbContext.Projects
            .AsNoTracking()
            .Where(project => !project.IsArchived)
            .Include(project => project.ProjectManager)
            .Include(project => project.Members)
                .ThenInclude(member => member.User);

        if (!isAdmin)
        {
            query = query.Where(project =>
                project.ProjectManagerId == userId ||
                project.Members.Any(member => member.UserId == userId && member.IsActive));
        }

        return await query
            .OrderByDescending(project => project.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Project?> GetByIdAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectManager)
            .Include(project => project.Members)
                .ThenInclude(member => member.User)
            .Where(project => project.Id == id && !project.IsArchived);

        if (!isAdmin)
        {
            query = query.Where(project =>
                project.ProjectManagerId == userId ||
                project.Members.Any(member => member.UserId == userId && member.IsActive));
        }

        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<(bool Succeeded, string? Error, Project? Project)> CreateAsync(
        Project project,
        string? managerId,
        IEnumerable<string> memberIds,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateAsync(project, managerId, memberIds, cancellationToken);
        if (validationError is not null)
        {
            return (false, validationError, null);
        }

        project.ProjectCode = project.ProjectCode.Trim();
        project.Name = project.Name.Trim();
        project.ProjectManagerId = managerId;
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        project.Members = BuildMembers(memberIds, currentUserId);

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, project);
    }

    public async Task<(bool Succeeded, string? Error)> UpdateAsync(
        Project project,
        string? managerId,
        IEnumerable<string> memberIds,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Projects
            .Include(item => item.Members)
            .SingleOrDefaultAsync(item => item.Id == project.Id && !item.IsArchived, cancellationToken);

        if (existing is null)
        {
            return (false, "Không tìm thấy dự án.");
        }

        var validationError = await ValidateAsync(project, managerId, memberIds, cancellationToken, project.Id);
        if (validationError is not null)
        {
            return (false, validationError);
        }

        existing.ProjectCode = project.ProjectCode.Trim();
        existing.Name = project.Name.Trim();
        existing.Location = project.Location?.Trim();
        existing.ClientName = project.ClientName?.Trim();
        existing.StartDate = project.StartDate;
        existing.ExpectedEndDate = project.ExpectedEndDate;
        existing.Budget = project.Budget;
        existing.Status = project.Status;
        existing.ProjectManagerId = managerId;
        existing.UpdatedAt = DateTime.UtcNow;

        dbContext.ProjectMembers.RemoveRange(existing.Members);
        existing.Members = BuildMembers(memberIds, currentUserId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private async Task<string?> ValidateAsync(
        Project project,
        string? managerId,
        IEnumerable<string> memberIds,
        CancellationToken cancellationToken,
        int? currentProjectId = null)
    {
        if (project.ExpectedEndDate < project.StartDate)
        {
            return "Ngày kết thúc dự kiến không được trước ngày bắt đầu.";
        }

        if (project.Budget < 0)
        {
            return "Ngân sách không được âm.";
        }

        var normalizedCode = project.ProjectCode.Trim();
        var duplicateCode = await dbContext.Projects.AnyAsync(
            item => item.ProjectCode == normalizedCode &&
                    !item.IsArchived &&
                    item.Id != currentProjectId,
            cancellationToken);
        if (duplicateCode)
        {
            return "Mã dự án đã tồn tại.";
        }

        var ids = memberIds
            .Append(managerId ?? string.Empty)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var activeUsers = await dbContext.Users
            .Where(user => ids.Contains(user.Id) && user.IsActive)
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);

        if (activeUsers.Count != ids.Count)
        {
            return "Chỉ được phân công tài khoản đang hoạt động.";
        }

        return null;
    }

    private static List<ProjectMember> BuildMembers(
        IEnumerable<string> memberIds,
        string currentUserId)
    {
        return memberIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .Select(id => new ProjectMember
            {
                UserId = id,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            })
            .ToList();
    }
}
