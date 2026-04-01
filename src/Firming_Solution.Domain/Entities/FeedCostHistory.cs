namespace Firming_Solution.Domain.Entities;

public class FeedCostHistory : BaseEntity
{
    public int FeedTypeId { get; set; }
    public FeedType? FeedType { get; set; }
    public DateTime RecordedDate { get; set; }
    public decimal PricePerKg { get; set; }
    public string? Supplier { get; set; }
    public string? Notes { get; set; }
}
