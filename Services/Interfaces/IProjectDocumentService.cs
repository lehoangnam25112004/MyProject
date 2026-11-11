using ConstructionProjectManagement.Models;
using Microsoft.AspNetCore.Http;

namespace ConstructionProjectManagement.Services.Interfaces;

public interface IProjectDocumentService
{
    Task<(bool Succeeded, string? Error, ProjectDocument? Document)> UploadAsync(
        int projectId,
        int? contractId,
        string category,
        IFormFile file,
        string userId,
        CancellationToken cancellationToken = default);

    Task<(ProjectDocument? Document, string? FilePath)> GetDownloadAsync(
        int id,
        string userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
