using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class Batch : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public string BatchName { get; set; } = string.Empty;
    public BatchSpecies Species { get; set; }
    public string? Breed { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public int InitialCount { get; set; }
    public decimal? InitialWeight_kg { get; set; }
    public decimal PurchaseCost { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Planning;
    public bool IsEidTarget { get; set; } = false;
    public string? Notes { get; set; }

    public ICollection<DailyFeedLog> FeedLogs { get; set; } = new List<DailyFeedLog>();
    public ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
    public ICollection<MortalityLog> MortalityLogs { get; set; } = new List<MortalityLog>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
    public ICollection<EidBazarPlan> EidBazarPlans { get; set; } = new List<EidBazarPlan>();
    public ICollection<AnimalCostBreakdown> AnimalCostBreakdowns { get; set; } = new List<AnimalCostBreakdown>();
}
