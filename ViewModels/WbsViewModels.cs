using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class WbsIndexViewModel
{
    public required Project Project { get; init; }
}

public sealed class ProjectPhaseViewModel
{
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên giai đoạn.")]
    [Display(Name = "Tên giai đoạn")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Thứ tự không hợp lệ.")]
    [Display(Name = "Thứ tự")]
    public int SequenceOrder { get; set; }
}

public sealed class WorkItemViewModel
{
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giai đoạn.")]
    [Display(Name = "Giai đoạn")]
    public int ProjectPhaseId { get; set; }

    [Display(Name = "Công việc cha")]
    public int? ParentWorkItemId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã WBS.")]
    [Display(Name = "Mã WBS")]
    public string WbsCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên hạng mục.")]
    [Display(Name = "Tên hạng mục")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Thứ tự")]
    public int SequenceOrder { get; set; }

    [Display(Name = "Trạng thái")]
    public WbsItemStatus Status { get; set; } = WbsItemStatus.Planned;
}
