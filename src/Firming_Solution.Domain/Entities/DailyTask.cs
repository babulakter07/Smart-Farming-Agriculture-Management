using Firming_Solution.Domain.Enums;

namespace Firming_Solution.Domain.Entities;

public class DailyTask : BaseEntity
{
    public int FarmId { get; set; }
    public Farm? Farm { get; set; }
    public string? AssignedToId { get; set; }
    public AppUser? AssignedTo { get; set; }
    public DateTime TaskDate { get; set; }
    public TaskType TaskType { get; set; }
    public string? Description { get; set; }
    public TimeSpan? StartTime { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
    public DateTime? CompletedAt { get; set; }
}
