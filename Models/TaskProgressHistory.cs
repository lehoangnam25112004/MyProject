using TaskEntity = ConstructionProjectManagement.Models.Task;

namespace ConstructionProjectManagement.Models;

public class TaskProgressHistory
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;

    public decimal PreviousPercentComplete { get; set; }

    public decimal NewPercentComplete { get; set; }

    public TaskStatus PreviousStatus { get; set; }

    public TaskStatus NewStatus { get; set; }

    public string? DelayReason { get; set; }

    public required string UpdatedById { get; set; }

    public ApplicationUser UpdatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
