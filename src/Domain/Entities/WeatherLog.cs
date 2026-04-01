using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class WeatherLog : BaseEntity
{
    public int FarmId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal? TempMax_C { get; set; }
    public decimal? TempMin_C { get; set; }
    public decimal? Humidity_Pct { get; set; }
    public decimal? Rainfall_mm { get; set; }
    public WeatherCondition? WeatherCondition { get; set; }
    public decimal? FeedDemandIndex { get; set; }

    public Farm Farm { get; set; } = null!;
}
