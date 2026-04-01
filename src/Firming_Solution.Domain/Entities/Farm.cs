using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class Farm : BaseEntity
{
    public string OwnerId { get; set; } = string.Empty;
    public AppUser? Owner { get; set; }
    public string FarmName { get; set; } = string.Empty;
    public FarmType FarmType { get; set; } = FarmType.Mixed;
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? TotalArea { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<LandParcel> LandParcels { get; set; } = new List<LandParcel>();
    public ICollection<WeatherLog> WeatherLogs { get; set; } = new List<WeatherLog>();
    public ICollection<EidBazarPlan> EidBazarPlans { get; set; } = new List<EidBazarPlan>();
    public ICollection<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
    public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
    public ICollection<AIRecommendation> AIRecommendations { get; set; } = new List<AIRecommendation>();
    public ICollection<UserFarm> UserFarms { get; set; } = new List<UserFarm>();
}
