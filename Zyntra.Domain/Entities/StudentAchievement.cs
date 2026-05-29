using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class StudentAchievement : EntityBase
{
    public long StudentId { get; set; }
    public string AchievementKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }

    public Student Student { get; set; } = null!;
}
