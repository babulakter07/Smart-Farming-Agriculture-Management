using System.ComponentModel.DataAnnotations;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Application.DTOs;

public class CostListDto
{
    public int Id { get; set; }
    public CostCategory CostCategory { get; set; }
    public string? Description { get; set; }
    public DateTime CostDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsActual { get; set; }
    public string? BatchName { get; set; }
    public string? CropName { get; set; }
}

public class CostCreateDto
{
    public int FarmId { get; set; }
    public int? BatchId { get; set; }
    public int? CropSeasonId { get; set; }
    public CostCategory CostCategory { get; set; }
    [MaxLength(300)]
    public string? Description { get; set; }
    [Required]
    public DateTime CostDate { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    public bool IsActual { get; set; } = true;
}

public class CostEditDto : CostCreateDto
{
    public int Id { get; set; }
}
