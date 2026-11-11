using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class MaterialEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã vật tư.")]
    public string MaterialCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên vật tư.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập đơn vị tính.")]
    public string Unit { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Tồn kho không hợp lệ.")]
    public decimal CurrentStock { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed class MaterialRequestCreateViewModel
{
    [Required]
    public int ProjectId { get; set; }

    public int? TaskId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MinLength(1, ErrorMessage = "Phiếu phải có ít nhất một vật tư.")]
    public List<MaterialRequestLineViewModel> Items { get; set; } = [];
}

public sealed class MaterialRequestLineViewModel
{
    [Required]
    public int MaterialId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal QuantityRequested { get; set; }
}

public sealed class MaterialIndexViewModel
{
    public required IReadOnlyCollection<Material> Materials { get; init; }
    public required IReadOnlyCollection<MaterialRequest> Requests { get; init; }
}
