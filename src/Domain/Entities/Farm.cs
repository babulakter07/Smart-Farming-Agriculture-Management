using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class Farm : BaseEntity
{
    public string OwnerId { get; set; } = null!;
    public string FarmName { get; set; } = null!;
    public FarmType FarmType { get; set; }
    public string? Location { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? TotalArea { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
    public ICollection<LandParcel> LandParcels { get; set; } = new List<LandParcel>();
    public ICollection<WeatherLog> WeatherLogs { get; set; } = new List<WeatherLog>();
    public ICollection<DailyTask> DailyTasks { get; set; } = new List<DailyTask>();
    public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    public ICollection<EidBazarPlan> EidBazarPlans { get; set; } = new List<EidBazarPlan>();
    public ICollection<Cost> Costs { get; set; } = new List<Cost>();
    public ICollection<AiRecommendation> AiRecommendations { get; set; } = new List<AiRecommendation>();
    public ICollection<UserFarm> UserFarms { get; set; } = new List<UserFarm>();
}
