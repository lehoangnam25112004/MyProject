using System.ComponentModel.DataAnnotations;
using ConstructionProjectManagement.Models;

namespace ConstructionProjectManagement.ViewModels;

public sealed class ContractEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn dự án.")]
    [Display(Name = "Dự án")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số hợp đồng.")]
    [StringLength(50)]
    [Display(Name = "Số hợp đồng")]
    public string ContractNumber { get; set; } = string.Empty;

    [Display(Name = "Chủ đầu tư")]
    public string? ClientName { get; set; }

    [Display(Name = "Nhà thầu")]
    public string? ContractorName { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá trị không được âm.")]
    [Display(Name = "Giá trị hợp đồng")]
    public decimal ContractValue { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày ký")]
    public DateTime? SignedDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày hiệu lực")]
    public DateTime? EffectiveDate { get; set; }

    [Display(Name = "Trạng thái")]
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
}

public sealed class ContractListViewModel
{
    public required IReadOnlyCollection<Contract> Contracts { get; init; }
}

public sealed class ContractDetailsViewModel
{
    public required Contract Contract { get; init; }
}
