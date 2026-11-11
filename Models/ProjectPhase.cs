using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ProjectPhase
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    [MaxLength(200)]
    public required string Name { get; set; }

    public int SequenceOrder { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Planned";

    public ICollection<WorkItem> WorkItems { get; set; } = [];
}
