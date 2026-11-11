using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class InspectionRequest
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; } = null!;
    [MaxLength(4000)] public string? ResultNotes { get; set; }
    [MaxLength(4000)] public string? CorrectiveAction { get; set; }
    public InspectionResult Result { get; set; } = InspectionResult.Pending;
    public required string RequestedById { get; set; }
    public ApplicationUser RequestedBy { get; set; } = null!;
    public string? InspectedById { get; set; }
    public ApplicationUser? InspectedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? InspectedAt { get; set; }
    public ICollection<InspectionHistory> History { get; set; } = [];
}
