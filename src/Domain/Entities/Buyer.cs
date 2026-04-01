using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class Buyer : BaseEntity
{
    public int FarmId { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public Farm Farm { get; set; } = null!;
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
