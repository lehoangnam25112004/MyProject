using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class EquipmentEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã thiết bị.")]
    public string EquipmentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên thiết bị.")]
    public string Name { get; set; } = string.Empty;

    public string? Category { get; set; }

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class EquipmentAssignmentViewModel
{
    [Required]
    public int EquipmentId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public int? TaskId { get; set; }

    [Required]
    public string AssignedToId { get; set; } = string.Empty;

    public string? Notes { get; set; }
}

public sealed class EquipmentIndexViewModel
{
    public required IReadOnlyCollection<Equipment> Equipment { get; init; }
    public required IReadOnlyCollection<EquipmentAssignment> Assignments { get; init; }
}
