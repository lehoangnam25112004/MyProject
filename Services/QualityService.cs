using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using TaskStatusModel = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.Services;

public sealed class QualityService(ApplicationDbContext dbContext) : IQualityService
{
    public async Task<(bool Succeeded, string? Error)> CreateIncidentAsync(
        IncidentCreateViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        var error = await ValidateProjectTaskAsync(model.ProjectId, model.TaskId, cancellationToken);
        if (error is not null) return (false, error);
        dbContext.Incidents.Add(new Incident
        {
            ProjectId = model.ProjectId, TaskId = model.TaskId, Title = model.Title.Trim(),
            Description = model.Description.Trim(), Priority = model.Priority,
            AssignedToId = model.AssignedToId, ReportedById = userId
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> CreateRfiAsync(
        RfiCreateViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        var error = await ValidateProjectTaskAsync(model.ProjectId, model.TaskId, cancellationToken);
        if (error is not null) return (false, error);
        dbContext.RfiRequests.Add(new RfiRequest
        {
            ProjectId = model.ProjectId, TaskId = model.TaskId, Subject = model.Subject.Trim(),
            Question = model.Question.Trim(), CreatedById = userId
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> RespondRfiAsync(
        int id, RfiRespondViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        var rfi = await dbContext.RfiRequests.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (rfi is null || rfi.Status == RfiStatus.Closed) return (false, "RFI không tồn tại hoặc đã đóng.");
        rfi.Response = model.Response.Trim();
        rfi.Status = RfiStatus.Answered;
        rfi.RespondedById = userId;
        rfi.RespondedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> CreateInspectionAsync(
        InspectionCreateViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.SingleOrDefaultAsync(item => item.Id == model.TaskId, cancellationToken);
        if (task is null || !task.RequiresInspection || task.Status == TaskStatusModel.Cancelled)
            return (false, "Công việc không hợp lệ hoặc không yêu cầu nghiệm thu.");
        var pending = await dbContext.InspectionRequests.AnyAsync(
            item => item.TaskId == model.TaskId && item.Result == InspectionResult.Pending, cancellationToken);
        if (pending) return (false, "Công việc đã có yêu cầu nghiệm thu đang chờ xử lý.");
        dbContext.InspectionRequests.Add(new InspectionRequest
        {
            ProjectId = task.ProjectId, TaskId = task.Id, RequestedById = userId
        });
        task.Status = TaskStatusModel.PendingInspection;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> RecordInspectionAsync(
        int id, InspectionResultViewModel model, string userId, CancellationToken cancellationToken = default)
    {
        if (model.Result is InspectionResult.Pending)
            return (false, "Phải chọn kết quả đạt hoặc không đạt.");
        if (model.Result == InspectionResult.Failed
            && string.IsNullOrWhiteSpace(model.CorrectiveAction))
        {
            return (false, "Nghiệm thu không đạt phải có nội dung khắc phục.");
        }
        var inspection = await dbContext.InspectionRequests
            .Include(item => item.Task)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (inspection is null) return (false, "Không tìm thấy yêu cầu nghiệm thu.");
        inspection.Result = model.Result;
        inspection.ResultNotes = model.Notes?.Trim();
        inspection.CorrectiveAction = model.CorrectiveAction?.Trim();
        inspection.InspectedById = userId;
        inspection.InspectedAt = DateTime.UtcNow;
        inspection.History.Add(new InspectionHistory
        {
            Result = model.Result, Notes = model.Notes?.Trim(), InspectedById = userId
        });
        inspection.Task.Status = model.Result == InspectionResult.Passed
            ? TaskStatusModel.Completed : TaskStatusModel.NeedsRework;
        inspection.Task.PercentComplete = model.Result == InspectionResult.Passed
            ? 100 : inspection.Task.PercentComplete;
        inspection.Task.ActualEndDate = model.Result == InspectionResult.Passed
            ? DateTime.UtcNow : inspection.Task.ActualEndDate;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    private async Task<string?> ValidateProjectTaskAsync(
        int projectId, int? taskId, CancellationToken cancellationToken)
    {
        var projectExists = await dbContext.Projects.AnyAsync(
            project => project.Id == projectId && !project.IsArchived, cancellationToken);
        if (!projectExists) return "Dự án không hợp lệ.";
        if (taskId.HasValue && !await dbContext.Tasks.AnyAsync(
                task => task.Id == taskId && task.ProjectId == projectId, cancellationToken))
            return "Công việc không thuộc dự án.";
        return null;
    }
}
