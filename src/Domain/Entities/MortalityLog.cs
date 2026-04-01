using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class MortalityLog : BaseEntity
{
    public int BatchId { get; set; }
    public DateTime LogDate { get; set; }
    public int Count { get; set; }
    public string? Reason { get; set; }
    public decimal? EstimatedLoss { get; set; }

    public Batch Batch { get; set; } = null!;
}
