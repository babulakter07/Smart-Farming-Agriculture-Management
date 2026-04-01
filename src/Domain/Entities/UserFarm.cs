using Firming_Solution.Domain.Common;

namespace Firming_Solution.Domain.Entities;

public class UserFarm : BaseEntity
{
    public string UserId { get; set; } = null!;
    public int FarmId { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Farm Farm { get; set; } = null!;
}
