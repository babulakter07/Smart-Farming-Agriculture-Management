namespace Firming_Solution.Domain.Entities;

public class Sale : BaseEntity
{
    public int BatchId { get; set; }
    public Batch? Batch { get; set; }
    public DateTime SaleDate { get; set; }
    public int? Quantity { get; set; }
    public decimal? TotalWeight_kg { get; set; }
    public decimal PricePerKg { get; set; }
    public decimal TotalRevenue { get; set; }
    public string? BuyerName { get; set; }
    public bool IsEidSale { get; set; } = false;
    public string? Notes { get; set; }
}
