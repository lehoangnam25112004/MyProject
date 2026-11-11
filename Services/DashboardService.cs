using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class DashboardService(ApplicationDbContext dbContext)
{
    public async Task<DashboardViewModel> GetSummaryAsync(
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var projects = dbContext.Projects.Where(project => !project.IsArchived);
        if (!isAdmin)
        {
            projects = projects.Where(project =>
                project.ProjectManagerId == userId ||
                project.Members.Any(member => member.UserId == userId && member.IsActive));
        }

        var projectIds = await projects.Select(project => project.Id)
            .ToListAsync(cancellationToken);
        var tasks = dbContext.Tasks.Where(task => projectIds.Contains(task.ProjectId));
        var now = DateTime.UtcNow;
        return new DashboardViewModel
        {
            ActiveUserCount = await dbContext.Users.CountAsync(user => user.IsActive, cancellationToken),
            ProjectCount = projectIds.Count,
            ActiveProjectCount = await projects.CountAsync(
                project => project.Status == ConstructionProjectManagement.Models.ProjectStatus.InProgress, cancellationToken),
            CompletedProjectCount = await dbContext.Projects.CountAsync(
                project =>                 projectIds.Contains(project.Id) && project.Status == ConstructionProjectManagement.Models.ProjectStatus.Completed,
                cancellationToken),
            OpenTaskCount = await tasks.CountAsync(
                task => task.Status != ConstructionProjectManagement.Models.TaskStatus.Completed &&
                        task.Status != ConstructionProjectManagement.Models.TaskStatus.Cancelled, cancellationToken),
            OverdueTaskCount = await tasks.CountAsync(
                task => task.PlannedEndDate < now &&
                        task.Status != ConstructionProjectManagement.Models.TaskStatus.Completed &&
                        task.Status != ConstructionProjectManagement.Models.TaskStatus.Cancelled, cancellationToken),
            PendingMaterialRequestCount = await dbContext.MaterialRequests.CountAsync(
                request => projectIds.Contains(request.ProjectId) &&
                           request.Status == ConstructionProjectManagement.Models.MaterialRequestStatus.Pending, cancellationToken),
            PendingInspectionCount = await dbContext.InspectionRequests.CountAsync(
                inspection => projectIds.Contains(inspection.ProjectId) &&
                              inspection.Result == ConstructionProjectManagement.Models.InspectionResult.Pending, cancellationToken),
            TotalExpense = await dbContext.ProjectExpenses
                .Where(expense => projectIds.Contains(expense.ProjectId))
                .SumAsync(expense => (decimal?)expense.Amount, cancellationToken) ?? 0
        };
    }
}
