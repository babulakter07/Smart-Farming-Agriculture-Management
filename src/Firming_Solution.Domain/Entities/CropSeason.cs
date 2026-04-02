using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class CropSeason : BaseEntity
{
    public int LandId { get; set; }
    public LandParcel? Land { get; set; }
    public string CropName { get; set; } = string.Empty;
    public string? Variety { get; set; }
    public DateTime? SowDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public decimal? ExpectedYield_kg { get; set; }
    public decimal? ActualYield_kg { get; set; }
    public decimal? SeedCost { get; set; }
    public string? YieldUnit { get; set; } = "kg";   // "kg" or "mon"
    public decimal? SaleUnitPrice { get; set; }       // price per kg or per মণ
    public CropStatus Status { get; set; } = CropStatus.Planning;

    public ICollection<FertiliserPlan> FertiliserPlans { get; set; } = new List<FertiliserPlan>();
    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
}
