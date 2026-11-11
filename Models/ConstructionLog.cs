using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class ConstructionLog
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public DateTime LogDate { get; set; }
    public required string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; } = null!;
    [MaxLength(100)] public string? Weather { get; set; }
    [MaxLength(2000)] public string? Workforce { get; set; }
    [MaxLength(4000)] public string? WorkPerformed { get; set; }
    [MaxLength(4000)] public string? MaterialsUsed { get; set; }
    [MaxLength(4000)] public string? EquipmentUsed { get; set; }
    [MaxLength(4000)] public string? Issues { get; set; }
    [MaxLength(260)] public string? AttachmentFileName { get; set; }
    [MaxLength(500)] public string? AttachmentStorageName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
