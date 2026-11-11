using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class Incident
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int? TaskId { get; set; }
    public TaskEntity? Task { get; set; }
    [MaxLength(200)] public required string Title { get; set; }
    [MaxLength(4000)] public required string Description { get; set; }
    public IncidentPriority Priority { get; set; }
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public string? AssignedToId { get; set; }
    public ApplicationUser? AssignedTo { get; set; }
    [MaxLength(4000)] public string? Resolution { get; set; }
    public required string ReportedById { get; set; }
    public ApplicationUser ReportedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}
