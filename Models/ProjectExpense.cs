using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ProjectExpense
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    [MaxLength(100)] public required string Category { get; set; }
    [MaxLength(300)] public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public required string RecordedById { get; set; }
    public ApplicationUser RecordedBy { get; set; } = null!;
}
