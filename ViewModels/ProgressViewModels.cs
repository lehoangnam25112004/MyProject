using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;
using TaskEntity = ConstructionProjectManagement.Models.Task;
using TaskStatusModel = ConstructionProjectManagement.Models.TaskStatus;

namespace ConstructionProjectManagement.ViewModels;

public sealed class ProgressIndexViewModel
{
    public required IReadOnlyCollection<TaskEntity> Tasks { get; init; }
}

public sealed class ProgressUpdateViewModel
{
    public int TaskId { get; set; }

    [Range(0, 100, ErrorMessage = "Phần trăm phải từ 0 đến 100.")]
    [Display(Name = "Phần trăm hoàn thành")]
    public decimal PercentComplete { get; set; }

    [Display(Name = "Trạng thái")]
    public TaskStatusModel Status { get; set; }

    [Display(Name = "Nguyên nhân chậm tiến độ")]
    public string? DelayReason { get; set; }
}
