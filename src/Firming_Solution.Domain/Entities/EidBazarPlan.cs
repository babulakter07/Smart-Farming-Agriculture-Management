using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class EidBazarPlan : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public EidType EidType { get; set; } = EidType.EidUlAdha;
    public DateTime EidDate { get; set; }
    public int? TargetAnimals { get; set; }
    public decimal? TargetWeightPerAnimal { get; set; }
    public decimal? ExpectedPricePerKg { get; set; }
    public decimal? ExpectedRevenue { get; set; }
    public int? LinkedBatchId { get; set; }
    public Batch? LinkedBatch { get; set; }
}
