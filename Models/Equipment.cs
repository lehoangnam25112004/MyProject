using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Equipment
{
    public int Id { get; set; }

    [MaxLength(30)]
    public required string EquipmentCode { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<EquipmentAssignment> Assignments { get; set; } = [];
}
