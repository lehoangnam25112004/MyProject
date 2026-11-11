namespace ConstructionProjectManagement.Models;

public class TaskAssignment
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public Task Task { get; set; } = null!;

    public required string UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
