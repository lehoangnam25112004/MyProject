using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class IncidentCreateViewModel
{
    [Required] public int ProjectId { get; set; }
    public int? TaskId { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;
    public string? AssignedToId { get; set; }
}

public sealed class RfiCreateViewModel
{
    [Required] public int ProjectId { get; set; }
    public int? TaskId { get; set; }
    [Required] public string Subject { get; set; } = string.Empty;
    [Required] public string Question { get; set; } = string.Empty;
}

public sealed class RfiRespondViewModel
{
    [Required] public string Response { get; set; } = string.Empty;
}

public sealed class InspectionCreateViewModel
{
    [Required] public int TaskId { get; set; }
}

public sealed class InspectionResultViewModel
{
    public InspectionResult Result { get; set; }
    public string? Notes { get; set; }
    public string? CorrectiveAction { get; set; }
}

public sealed class QualityIndexViewModel
{
    public required IReadOnlyCollection<Incident> Incidents { get; init; }
    public required IReadOnlyCollection<RfiRequest> Rfis { get; init; }
    public required IReadOnlyCollection<InspectionRequest> Inspections { get; init; }
}
