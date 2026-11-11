using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Services;

public sealed class CompletionService(ApplicationDbContext dbContext) : ICompletionService
{
    public async Task<(bool Succeeded, string? Error)> AddExpenseAsync(ExpenseCreateViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Projects.AnyAsync(p => p.Id == model.ProjectId && !p.IsArchived, cancellationToken))
            return (false, "Dự án không hợp lệ.");
        dbContext.ProjectExpenses.Add(new ProjectExpense { ProjectId = model.ProjectId, Category = model.Category.Trim(), Description = model.Description.Trim(), Amount = model.Amount, ExpenseDate = model.ExpenseDate.Date, RecordedById = userId });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> CreateHandoverAsync(HandoverCreateViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects.SingleOrDefaultAsync(p => p.Id == model.ProjectId && !p.IsArchived, cancellationToken);
        if (project is null) return (false, "Dự án không hợp lệ.");
        var openTasks = await dbContext.Tasks.AnyAsync(t => t.ProjectId == model.ProjectId && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled, cancellationToken);
        if (openTasks) return (false, "Chưa thể bàn giao khi còn công việc chưa hoàn thành.");
        dbContext.ProjectHandovers.Add(new ProjectHandover { ProjectId = model.ProjectId, Title = model.Title.Trim(), Notes = model.Notes?.Trim(), ReceivedByName = model.ReceivedByName.Trim(), HandoverDate = model.HandoverDate.Date, HandedOverById = userId });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> ConfirmHandoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var handover = await dbContext.ProjectHandovers.SingleOrDefaultAsync(h => h.Id == id, cancellationToken);
        if (handover is null || handover.IsConfirmed) return (false, "Hồ sơ bàn giao không hợp lệ.");
        handover.IsConfirmed = true; handover.ConfirmedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> ArchiveProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects.SingleOrDefaultAsync(p => p.Id == projectId && !p.IsArchived, cancellationToken);
        if (project is null) return (false, "Không tìm thấy dự án.");
        var handover = await dbContext.ProjectHandovers.AnyAsync(h => h.ProjectId == projectId && h.IsConfirmed, cancellationToken);
        var openTasks = await dbContext.Tasks.AnyAsync(t => t.ProjectId == projectId && t.Status != TaskStatus.Completed && t.Status != TaskStatus.Cancelled, cancellationToken);
        var openIncidents = await dbContext.Incidents.AnyAsync(
            incident => incident.ProjectId == projectId
                && incident.Status != IncidentStatus.Resolved
                && incident.Status != IncidentStatus.Closed,
            cancellationToken);
        var openRfis = await dbContext.RfiRequests.AnyAsync(
            rfi => rfi.ProjectId == projectId && rfi.Status != RfiStatus.Closed,
            cancellationToken);
        var pendingInspections = await dbContext.InspectionRequests.AnyAsync(
            inspection => inspection.ProjectId == projectId
                && inspection.Result == InspectionResult.Pending,
            cancellationToken);
        if (!handover || openTasks || openIncidents || openRfis || pendingInspections)
        {
            return (false, "Dự án phải có bàn giao xác nhận và không còn công việc, sự cố, RFI hoặc nghiệm thu mở.");
        }
        project.IsArchived = true; project.Status = ProjectStatus.Completed; project.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }
}
