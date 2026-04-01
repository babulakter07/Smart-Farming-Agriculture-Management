using Firming_Solution.Domain.Common;
using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class DailyTask : BaseEntity
{
    public int FarmId { get; set; }
    public string? AssignedToId { get; set; }
    public DateTime TaskDate { get; set; }
    public TaskType TaskType { get; set; }
    public string? Description { get; set; }
    public TimeSpan? StartTime { get; set; }
    public Firming_Solution.Domain.Enums.TaskStatus Status { get; set; } = Firming_Solution.Domain.Enums.TaskStatus.Pending;
    public DateTime? CompletedAt { get; set; }

    public Farm Farm { get; set; } = null!;
    public ApplicationUser? AssignedTo { get; set; }
}
