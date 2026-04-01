namespace Firming_Solution.Domain.Entities;

public class FertiliserPlan : BaseEntity
{
    public int SeasonId { get; set; }
    public CropSeason? Season { get; set; }
    public int LandId { get; set; }
    public LandParcel? Land { get; set; }
    public string FertiliserName { get; set; } = string.Empty;
    public DateTime ApplyDate { get; set; }
    public decimal? DoseKgPerDecimal { get; set; }
    public decimal? TotalQuantity_kg { get; set; }
    public decimal? PricePerKg { get; set; }
    public decimal? TotalCost { get; set; }
    public bool IsApplied { get; set; } = false;
}
