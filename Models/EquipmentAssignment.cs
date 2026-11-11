using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class EquipmentAssignment
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public Equipment Equipment { get; set; } = null!;

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int? TaskId { get; set; }

    public TaskEntity? Task { get; set; }

    public required string AssignedToId { get; set; }

    public ApplicationUser AssignedTo { get; set; } = null!;

    public required string HandedOverById { get; set; }

    public ApplicationUser HandedOverBy { get; set; } = null!;

    public DateTime HandedOverAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReturnedAt { get; set; }

    public string? ReturnedById { get; set; }

    public ApplicationUser? ReturnedBy { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
