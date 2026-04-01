using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class CropSeason : BaseEntity
{
    public int LandId { get; set; }
    public string CropName { get; set; } = null!;
    public string? Variety { get; set; }
    public DateTime? SowDate { get; set; }
    public DateTime? ExpectedHarvestDate { get; set; }
    public DateTime? ActualHarvestDate { get; set; }
    public decimal? ExpectedYield_kg { get; set; }
    public decimal? ActualYield_kg { get; set; }
    public decimal? SeedCost { get; set; }
    public CropSeasonStatus Status { get; set; } = CropSeasonStatus.Planning;

    public LandParcel LandParcel { get; set; } = null!;
    public ICollection<FertiliserPlan> FertiliserPlans { get; set; } = new List<FertiliserPlan>();
    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
}
