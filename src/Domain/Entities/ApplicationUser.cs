using Firming_Solution.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Firming_Solution.Domain.Entities
{
    // Ensure Microsoft.AspNetCore.Identity.EntityFrameworkCore is referenced in the project
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public UserRole Role { get; set; } = UserRole.Viewer;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Farm> OwnedFarms { get; set; } = new List<Farm>();
        public ICollection<UserFarm> UserFarms { get; set; } = new List<UserFarm>();
        public ICollection<DailyTask> AssignedTasks { get; set; } = new List<DailyTask>();
        public ICollection<Investment> RecordedInvestments { get; set; } = new List<Investment>();
        public ICollection<DailyFeedLog> FeedLogs { get; set; } = new List<DailyFeedLog>();
        public ICollection<Cost> EnteredCosts { get; set; } = new List<Cost>();
    }
}
