using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Models;
using ConstructionProjectManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ConstructionProjectManagement.Services;

public sealed class ProjectDocumentService(
    ApplicationDbContext dbContext,
    IWebHostEnvironment environment) : IProjectDocumentService
{
    private const long MaxFileSize = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg"
    };

    public async Task<(bool Succeeded, string? Error, ProjectDocument? Document)> UploadAsync(
        int projectId,
        int? contractId,
        string category,
        IFormFile file,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0 || file.Length > MaxFileSize)
        {
            return (false, "Tệp phải lớn hơn 0 và không vượt quá 10 MB.", null);
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            return (false, "Định dạng tệp không được hỗ trợ.", null);
        }

        var project = await dbContext.Projects
            .Include(item => item.Members)
            .SingleOrDefaultAsync(item => item.Id == projectId && !item.IsArchived, cancellationToken);
        if (project is null)
        {
            return (false, "Dự án không hợp lệ.", null);
        }

        if (contractId.HasValue)
        {
            var contractExists = await dbContext.Contracts.AnyAsync(
                contract => contract.Id == contractId &&
                            contract.ProjectId == projectId &&
                            !contract.IsArchived,
                cancellationToken);
            if (!contractExists)
            {
                return (false, "Hợp đồng không thuộc dự án đã chọn.", null);
            }
        }

        if (project.ProjectManagerId != userId &&
            !project.Members.Any(member => member.UserId == userId && member.IsActive))
        {
            return (false, "Bạn không có quyền trên dự án này.", null);
        }

        var storageDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "ProjectDocuments");
        Directory.CreateDirectory(storageDirectory);
        var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(storageDirectory, storedFileName);

        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var document = new ProjectDocument
        {
            ProjectId = projectId,
            ContractId = contractId,
            FileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            Category = category.Trim(),
            UploadedById = userId,
            UploadedAt = DateTime.UtcNow
        };

        dbContext.ProjectDocuments.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, document);
    }

    public async Task<(ProjectDocument? Document, string? FilePath)> GetDownloadAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var document = await dbContext.ProjectDocuments
            .Include(item => item.Project)
                .ThenInclude(project => project.Members)
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsArchived, cancellationToken);
        if (document is null ||
            (!isAdmin &&
             document.Project.ProjectManagerId != userId &&
             !document.Project.Members.Any(member => member.UserId == userId && member.IsActive)))
        {
            return (null, null);
        }

        var filePath = Path.Combine(
            environment.ContentRootPath,
            "App_Data",
            "ProjectDocuments",
            document.StoredFileName);
        return File.Exists(filePath) ? (document, filePath) : (null, null);
    }
}
