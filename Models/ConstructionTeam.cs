using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ConstructionTeam
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<TeamMember> Members { get; set; } = [];
}
