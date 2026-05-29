using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class WorkoutSession : EntityBase
{
    public long StudentId { get; set; }
    public string WorkoutDay { get; set; } = "A";
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public long? CheckInId { get; set; }

    public Student Student { get; set; } = null!;
    public ICollection<ExerciseLog> ExerciseLogs { get; set; } = [];
}
