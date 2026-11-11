using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class MaterialTransaction
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public Material Material { get; set; } = null!;

    public int? MaterialRequestId { get; set; }

    public MaterialRequest? MaterialRequest { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int? TaskId { get; set; }

    public TaskEntity? Task { get; set; }

    public decimal Quantity { get; set; }

    public MaterialTransactionType Type { get; set; }

    public required string PerformedById { get; set; }

    public ApplicationUser PerformedBy { get; set; } = null!;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
