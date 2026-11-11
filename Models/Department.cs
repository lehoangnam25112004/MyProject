using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Department
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];
}
