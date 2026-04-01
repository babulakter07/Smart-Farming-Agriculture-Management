using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class LandParcel : BaseEntity
{
    public int FarmId { get; set; }
    public string LandName { get; set; } = null!;
    public decimal Area_Decimal { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public decimal? LeaseCostPerSeason { get; set; }
    public string? SoilType { get; set; }
    public DateTime? LastTestedDate { get; set; }

    public Farm Farm { get; set; } = null!;
    public ICollection<CropSeason> CropSeasons { get; set; } = new List<CropSeason>();
}
