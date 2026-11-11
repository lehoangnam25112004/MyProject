using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class WorkItem
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int ProjectPhaseId { get; set; }

    public ProjectPhase ProjectPhase { get; set; } = null!;

    public int? ParentWorkItemId { get; set; }

    public WorkItem? ParentWorkItem { get; set; }

    public ICollection<WorkItem> Children { get; set; } = [];

    [MaxLength(30)]
    public required string WbsCode { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    public int Level { get; set; }

    public int SequenceOrder { get; set; }

    public WbsItemStatus Status { get; set; } = WbsItemStatus.Planned;
}
