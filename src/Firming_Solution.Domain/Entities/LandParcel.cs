using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class LandParcel : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public string LandName { get; set; } = string.Empty;
    public decimal Area_Decimal { get; set; }
    public OwnershipType OwnershipType { get; set; } = OwnershipType.Own;
    public decimal? LeaseCostPerSeason { get; set; }
    public string? SoilType { get; set; }
    public DateTime? LastTestedDate { get; set; }

    public ICollection<CropSeason> CropSeasons { get; set; } = new List<CropSeason>();
    public ICollection<FertiliserPlan> FertiliserPlans { get; set; } = new List<FertiliserPlan>();
}
