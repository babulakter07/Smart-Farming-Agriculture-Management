namespace Firming_Solution.Domain.Entities;

public class AnimalCostBreakdown : BaseEntity
{
    public int BatchId { get; set; }
    public Batch? Batch { get; set; }
    public string? AnimalTagNo { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? PurchaseWeightKg { get; set; }
    public decimal? AllocatedFeedCost { get; set; }
    public decimal? AllocatedMedCost { get; set; }
    public decimal? DirectMedCost { get; set; }
    public decimal? TotalCostThisAnimal { get; set; }
    public DateTime? SaleDate { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? SaleWeightKg { get; set; }
    public decimal? ProfitLoss { get; set; }
    public decimal? FCR_Individual { get; set; }
}
