using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class WeightLog : BaseEntity
{
    public int BatchId { get; set; }
    public DateTime LogDate { get; set; }
    public int? SampleCount { get; set; }
    public decimal AvgWeight_kg { get; set; }
    public decimal? TotalEstWeight { get; set; }
    public decimal? FCR_Cumulative { get; set; }
    public string? Notes { get; set; }

    public Batch Batch { get; set; } = null!;
}
