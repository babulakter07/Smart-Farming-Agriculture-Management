using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class Sale : BaseEntity
{
    public int BatchId { get; set; }
    public int? BuyerId { get; set; }
    public DateTime SaleDate { get; set; }
    public int? Quantity { get; set; }
    public decimal? TotalWeight_kg { get; set; }
    public decimal PricePerKg { get; set; }
    public decimal TotalRevenue { get; set; }
    public bool IsEidSale { get; set; } = false;
    public string? Notes { get; set; }

    public Batch Batch { get; set; } = null!;
    public Buyer? Buyer { get; set; }
}
