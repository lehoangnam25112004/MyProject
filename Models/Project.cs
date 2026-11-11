using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Project
{
    public int Id { get; set; }

    [MaxLength(30)]
    public required string ProjectCode { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(300)]
    public string? Location { get; set; }

    [MaxLength(200)]
    public string? ClientName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpectedEndDate { get; set; }

    public decimal Budget { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    public string? ProjectManagerId { get; set; }

    public ApplicationUser? ProjectManager { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsArchived { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = [];

    public ICollection<ProjectPhase> Phases { get; set; } = [];
}
