using System.ComponentModel.DataAnnotations;
using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class MaterialRequest
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int? TaskId { get; set; }

    public TaskEntity? Task { get; set; }

    public required string RequestedById { get; set; }

    public ApplicationUser RequestedBy { get; set; } = null!;

    public string? ApprovedById { get; set; }

    public ApplicationUser? ApprovedBy { get; set; }

    public MaterialRequestStatus Status { get; set; } = MaterialRequestStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public ICollection<MaterialRequestItem> Items { get; set; } = [];

    public ICollection<MaterialTransaction> Transactions { get; set; } = [];
}
