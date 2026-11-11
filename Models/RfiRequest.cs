using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class RfiRequest
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int? TaskId { get; set; }
    public TaskEntity? Task { get; set; }
    [MaxLength(200)] public required string Subject { get; set; }
    [MaxLength(4000)] public required string Question { get; set; }
    [MaxLength(4000)] public string? Response { get; set; }
    public RfiStatus Status { get; set; } = RfiStatus.Open;
    public required string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; } = null!;
    public string? RespondedById { get; set; }
    public ApplicationUser? RespondedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}
