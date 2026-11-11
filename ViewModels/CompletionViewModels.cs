using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class ExpenseCreateViewModel
{
    [Required] public int ProjectId { get; set; }
    [Required] public string Category { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [DataType(DataType.Date)] public DateTime ExpenseDate { get; set; } = DateTime.Today;
}

public sealed class HandoverCreateViewModel
{
    [Required] public int ProjectId { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    [Required] public string ReceivedByName { get; set; } = string.Empty;
    [DataType(DataType.Date)] public DateTime HandoverDate { get; set; } = DateTime.Today;
}

public sealed class CompletionIndexViewModel
{
    public required IReadOnlyCollection<ProjectExpense> Expenses { get; init; }
    public required IReadOnlyCollection<ProjectHandover> Handovers { get; init; }
    public required IReadOnlyCollection<Project> Projects { get; init; }
    public required IReadOnlyCollection<Models.Task> OpenTasks { get; init; }
    public decimal TotalExpense { get; init; }
}
