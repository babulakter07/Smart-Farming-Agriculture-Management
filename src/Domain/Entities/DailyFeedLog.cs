using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class DailyFeedLog : BaseEntity
{
    public int BatchId { get; set; }
    public int FeedTypeId { get; set; }
    public string? LoggedById { get; set; }
    public DateTime LogDate { get; set; }
    public decimal Quantity_kg { get; set; }
    public decimal PricePerKg { get; set; }
    public decimal TotalCost => Quantity_kg * PricePerKg;
    public FeedSession Session { get; set; }

    public Batch Batch { get; set; } = null!;
    public FeedType FeedType { get; set; } = null!;
    public ApplicationUser? LoggedBy { get; set; }
}
