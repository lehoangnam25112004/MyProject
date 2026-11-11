using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ProjectHandover
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    [MaxLength(200)] public required string Title { get; set; }
    [MaxLength(4000)] public string? Notes { get; set; }
    public required string HandedOverById { get; set; }
    public ApplicationUser HandedOverBy { get; set; } = null!;
    [MaxLength(200)] public required string ReceivedByName { get; set; }
    public DateTime HandoverDate { get; set; } = DateTime.Today;
    public bool IsConfirmed { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
