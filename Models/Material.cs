using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Material
{
    public int Id { get; set; }

    [MaxLength(30)]
    public required string MaterialCode { get; set; }

    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(30)]
    public required string Unit { get; set; }

    public decimal CurrentStock { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MaterialRequestItem> RequestItems { get; set; } = [];

    public ICollection<MaterialTransaction> Transactions { get; set; } = [];
}
