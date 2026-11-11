using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Employee
{
    public int Id { get; set; }

    [MaxLength(30)]
    public required string EmployeeCode { get; set; }

    public required string UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public Department? Department { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    public DateTime? HireDate { get; set; }

    public bool IsActive { get; set; } = true;
}
