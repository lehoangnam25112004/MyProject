using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class EmployeeEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã nhân viên.")]
    [Display(Name = "Mã nhân viên")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn tài khoản.")]
    [Display(Name = "Tài khoản")]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Bộ phận")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Chức vụ")]
    public string? Position { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày vào làm")]
    public DateTime? HireDate { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; } = true;
}

public sealed class EmployeeListViewModel
{
    public required IReadOnlyCollection<Employee> Employees { get; init; }
}

public sealed class ProjectMemberViewModel
{
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn tài khoản.")]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Vai trò trong dự án")]
    public string? AssignmentRole { get; set; }
}
