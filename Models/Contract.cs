using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models;

public class Contract
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;

    [MaxLength(50)]
    public required string ContractNumber { get; set; }

    [MaxLength(200)]
    public string? ClientName { get; set; }

    [MaxLength(200)]
    public string? ContractorName { get; set; }

    public decimal ContractValue { get; set; }

    public DateTime? SignedDate { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsArchived { get; set; }

    public ICollection<ProjectDocument> Documents { get; set; } = [];
}
