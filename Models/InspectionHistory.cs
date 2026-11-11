using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class InspectionHistory
{
    public int Id { get; set; }
    public int InspectionRequestId { get; set; }
    public InspectionRequest InspectionRequest { get; set; } = null!;
    public InspectionResult Result { get; set; }
    [MaxLength(4000)] public string? Notes { get; set; }
    public required string InspectedById { get; set; }
    public ApplicationUser InspectedBy { get; set; } = null!;
    public DateTime InspectedAt { get; set; } = DateTime.UtcNow;
}
