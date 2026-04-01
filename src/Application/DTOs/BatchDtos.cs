using System.ComponentModel.DataAnnotations;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Application.DTOs;

public class BatchListDto
{
    public int Id { get; set; }
    public string BatchName { get; set; } = null!;
    public BatchSpecies Species { get; set; }
    public string? Breed { get; set; }
    public DateTime StartDate { get; set; }
    public int InitialCount { get; set; }
    public BatchStatus Status { get; set; }
    public bool IsEidTarget { get; set; }
    public decimal PurchaseCost { get; set; }
    public int LiveCount { get; set; }
}

public class BatchCreateDto
{
    public int FarmId { get; set; }
    [Required, MaxLength(100)]
    public string BatchName { get; set; } = null!;
    public BatchSpecies Species { get; set; }
    public string? Breed { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    [Range(1, int.MaxValue)]
    public int InitialCount { get; set; }
    public decimal? InitialWeight_kg { get; set; }
    [Range(0, double.MaxValue)]
    public decimal PurchaseCost { get; set; }
    public bool IsEidTarget { get; set; }
    public string? Notes { get; set; }
}

public class BatchEditDto : BatchCreateDto
{
    public int Id { get; set; }
    public BatchStatus Status { get; set; }
}

public class BatchDetailDto : BatchEditDto
{
    public string FarmName { get; set; } = null!;
    public decimal TotalFeedCost { get; set; }
    public decimal TotalOtherCost { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal? CurrentFCR { get; set; }
    public decimal? CurrentAvgWeight { get; set; }
    public int LiveCount { get; set; }
    public int TotalMortalities { get; set; }
}
