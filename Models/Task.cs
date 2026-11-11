using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Task
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public int? WorkItemId { get; set; }

    public WorkItem? WorkItem { get; set; }

    public int? ParentTaskId { get; set; }

    public Task? ParentTask { get; set; }

    public ICollection<Task> Children { get; set; } = [];

    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime PlannedStartDate { get; set; }

    public DateTime PlannedEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public decimal PercentComplete { get; set; }

    public bool RequiresInspection { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskAssignment> Assignments { get; set; } = [];

    public ICollection<TaskDependency> Dependencies { get; set; } = [];
}
