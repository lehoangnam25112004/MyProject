using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class ProjectEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã dự án.")]
    [StringLength(30)]
    [Display(Name = "Mã dự án")]
    public string ProjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên dự án.")]
    [StringLength(200)]
    [Display(Name = "Tên dự án")]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Địa điểm")]
    public string? Location { get; set; }

    [StringLength(200)]
    [Display(Name = "Chủ đầu tư")]
    public string? ClientName { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc dự kiến")]
    public DateTime ExpectedEndDate { get; set; } = DateTime.Today.AddMonths(1);

    [Range(0, double.MaxValue, ErrorMessage = "Ngân sách không được âm.")]
    [Display(Name = "Ngân sách")]
    public decimal Budget { get; set; }

    [Display(Name = "Trạng thái")]
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

    [Display(Name = "Project Manager")]
    public string? ProjectManagerId { get; set; }

    [Display(Name = "Thành viên dự án")]
    public List<string> MemberIds { get; set; } = [];
}

public sealed class ProjectListViewModel
{
    public required IReadOnlyCollection<Project> Projects { get; init; }
}
