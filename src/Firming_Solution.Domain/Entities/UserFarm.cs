namespace Firming_Solution.Domain.Entities;

public class UserFarm : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public AppUser? User { get; set; }
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
}
