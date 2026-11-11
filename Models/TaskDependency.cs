namespace ConstructionProjectManagement.Models;

public class TaskDependency
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public Task Task { get; set; } = null!;

    public int PredecessorTaskId { get; set; }

    public Task PredecessorTask { get; set; } = null!;
}
