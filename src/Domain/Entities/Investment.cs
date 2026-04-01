using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class Investment : BaseEntity
{
    public int FarmId { get; set; }
    public string? RecordedById { get; set; }
    public DateTime InvestDate { get; set; }
    public string? Category { get; set; }
    public decimal Amount { get; set; }
    public InvestmentSource? Source { get; set; }
    public string? Description { get; set; }

    public Farm Farm { get; set; } = null!;
    public ApplicationUser? RecordedBy { get; set; }
}
