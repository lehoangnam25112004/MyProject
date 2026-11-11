using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ProjectDocument
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int? ContractId { get; set; }

    public Contract? Contract { get; set; }

    [MaxLength(200)]
    public required string FileName { get; set; }

    [MaxLength(200)]
    public required string StoredFileName { get; set; }

    [MaxLength(100)]
    public required string ContentType { get; set; }

    public long FileSize { get; set; }

    [MaxLength(100)]
    public required string Category { get; set; }

    public string UploadedById { get; set; } = null!;

    public ApplicationUser UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsArchived { get; set; }
}
