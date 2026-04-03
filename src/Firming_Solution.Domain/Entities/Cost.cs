namespace Firming_Solution.Domain.Entities;

public class Cost : BaseEntity
{
    public int? BatchId { get; set; }
    public Batch? Batch { get; set; }
    public int? CropSeasonId { get; set; }
    public CropSeason? CropSeason { get; set; }
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public string CostCategory { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string? Description { get; set; }
    public DateTime CostDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsActual { get; set; } = true;
    public string? EnteredById { get; set; }
    public AppUser? EnteredBy { get; set; }
}
