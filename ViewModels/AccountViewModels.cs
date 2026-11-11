using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.ViewModels;

public sealed class AccountListItemViewModel
{
    public required string Id { get; init; }

    public required string UserName { get; init; }

    public string? FullName { get; init; }

    public bool IsActive { get; init; }

    public required IReadOnlyCollection<string> Roles { get; init; }
}

public sealed class AccountListViewModel
{
    public required IReadOnlyCollection<AccountListItemViewModel> Accounts { get; init; }
}

public sealed class AccountEditViewModel
{
    public string? Id { get; init; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [Display(Name = "Tên đăng nhập")]
    public string UserName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Vai trò")]
    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
    [Display(Name = "Mật khẩu")]
    public string? Password { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool IsActive { get; set; } = true;
}

public sealed class ProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }
}
