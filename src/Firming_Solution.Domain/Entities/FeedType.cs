using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class FeedType : BaseEntity
{
    public string FeedName { get; set; } = string.Empty;
    public FeedCategory Category { get; set; }
    public string? Manufacturer { get; set; }
    public string? Unit { get; set; } = "kg";
    public decimal? CurrentPrice { get; set; }
    public DateTime? LastUpdated { get; set; }

    public ICollection<DailyFeedLog> DailyFeedLogs { get; set; } = new List<DailyFeedLog>();
    public ICollection<FeedCostHistory> PriceHistory { get; set; } = new List<FeedCostHistory>();
}
