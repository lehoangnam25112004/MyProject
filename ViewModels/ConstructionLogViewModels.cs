using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;
using Microsoft.AspNetCore.Http;

namespace ConstructionProjectManagement.ViewModels;

public sealed class ConstructionLogEditViewModel
{
    public int Id { get; set; }
    [Required] public int ProjectId { get; set; }
    [DataType(DataType.Date)] public DateTime LogDate { get; set; } = DateTime.Today;
    [Required(ErrorMessage = "Vui lòng nhập điều kiện thời tiết.")] public string Weather { get; set; } = string.Empty;
    public string? Workforce { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập công việc đã thực hiện.")] public string WorkPerformed { get; set; } = string.Empty;
    public string? MaterialsUsed { get; set; }
    public string? EquipmentUsed { get; set; }
    public string? Issues { get; set; }
    public IFormFile? Attachment { get; set; }
}

public sealed class ConstructionLogIndexViewModel
{
    public required IReadOnlyCollection<ConstructionLog> Logs { get; init; }
    public required IReadOnlyCollection<Project> Projects { get; init; }
}
