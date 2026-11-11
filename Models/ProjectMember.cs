using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ProjectMember
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    public required string UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    [MaxLength(100)]
    public string? AssignmentRole { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
