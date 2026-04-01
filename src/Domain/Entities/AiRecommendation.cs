using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class AiRecommendation : BaseEntity
{
    public int FarmId { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public AiRecoType RecoType { get; set; }
    public string RecoText { get; set; } = null!;
    public decimal? ConfidenceScore { get; set; }
    public DateTime? BasedOnDataFrom { get; set; }
    public bool IsActedUpon { get; set; } = false;

    public Farm Farm { get; set; } = null!;
}
