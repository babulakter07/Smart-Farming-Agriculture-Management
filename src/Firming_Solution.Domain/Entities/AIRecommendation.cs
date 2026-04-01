using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class AIRecommendation : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public RecommendationType RecoType { get; set; }
    public string RecoText { get; set; } = string.Empty;
    public decimal? ConfidenceScore { get; set; }
    public DateTime? BasedOnDataFrom { get; set; }
    public bool IsActedUpon { get; set; } = false;
}
