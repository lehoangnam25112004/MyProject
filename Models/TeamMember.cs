namespace ConstructionProjectManagement.Models;

public class TeamMember
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public ConstructionTeam Team { get; set; } = null!;

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;
}
