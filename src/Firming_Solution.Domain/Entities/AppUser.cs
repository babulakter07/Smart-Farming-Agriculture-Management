using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Firming_Solution.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    public UserRole Role { get; set; } = UserRole.Viewer;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ICollection<Farm> OwnedFarms { get; set; } = new List<Farm>();
    public ICollection<UserFarm> UserFarms { get; set; } = new List<UserFarm>();
    public ICollection<DailyFeedLog> FeedLogs { get; set; } = new List<DailyFeedLog>();
    public ICollection<Cost> EnteredCosts { get; set; } = new List<Cost>();
    public ICollection<Investment> RecordedInvestments { get; set; } = new List<Investment>();
    public ICollection<DailyTask> AssignedTasks { get; set; } = new List<DailyTask>();
}
