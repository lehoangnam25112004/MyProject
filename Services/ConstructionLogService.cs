using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using ConstructionProjectManagement.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class ConstructionLogService(
    ApplicationDbContext dbContext,
    IWebHostEnvironment environment) : IConstructionLogService
{
    private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png"];

    public async Task<(bool Succeeded, string? Error)> SaveAsync(
        ConstructionLogEditViewModel model, string userId,
        CancellationToken cancellationToken = default)
    {
        var projectExists = await dbContext.Projects.AnyAsync(
            project => project.Id == model.ProjectId && !project.IsArchived, cancellationToken);
        if (!projectExists)
        {
            return (false, "Dự án không hợp lệ.");
        }

        if (model.Attachment is { Length: > 0 } file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || file.Length > 10 * 1024 * 1024)
            {
                return (false, "Ảnh đính kèm chỉ hỗ trợ JPG/PNG và tối đa 10 MB.");
            }
        }

        ConstructionLog log;
        if (model.Id == 0)
        {
            log = new ConstructionLog { ProjectId = model.ProjectId, CreatedById = userId };
            dbContext.ConstructionLogs.Add(log);
        }
        else
        {
            log = await dbContext.ConstructionLogs.SingleOrDefaultAsync(
                item => item.Id == model.Id, cancellationToken) ?? throw new InvalidOperationException("Không tìm thấy nhật ký.");
        }

        log.ProjectId = model.ProjectId;
        log.LogDate = model.LogDate.Date;
        log.Weather = model.Weather.Trim();
        log.Workforce = model.Workforce?.Trim();
        log.WorkPerformed = model.WorkPerformed.Trim();
        log.MaterialsUsed = model.MaterialsUsed?.Trim();
        log.EquipmentUsed = model.EquipmentUsed?.Trim();
        log.Issues = model.Issues?.Trim();

        if (model.Attachment is { Length: > 0 } attachment)
        {
            var directory = Path.Combine(environment.ContentRootPath, "App_Data", "ConstructionLogs");
            Directory.CreateDirectory(directory);
            var storageName = $"{Guid.NewGuid():N}{Path.GetExtension(attachment.FileName).ToLowerInvariant()}";
            await using var stream = File.Create(Path.Combine(directory, storageName));
            await attachment.CopyToAsync(stream, cancellationToken);
            log.AttachmentFileName = Path.GetFileName(attachment.FileName);
            log.AttachmentStorageName = storageName;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }
}
