using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class FeedType : BaseEntity
{
    public string FeedName { get; set; } = null!;
    public string? Category { get; set; }
    public string? Manufacturer { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal? CurrentPrice { get; set; }
    public DateTime? LastUpdated { get; set; }

    public ICollection<DailyFeedLog> DailyFeedLogs { get; set; } = new List<DailyFeedLog>();
}
