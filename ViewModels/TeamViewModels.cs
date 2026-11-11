using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.ViewModels;

public sealed class TeamEditViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đội.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public sealed class TeamMemberAddViewModel
{
    public int TeamId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn nhân viên.")]
    public int EmployeeId { get; set; }
}
