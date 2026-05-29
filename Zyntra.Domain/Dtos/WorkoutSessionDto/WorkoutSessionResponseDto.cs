namespace Zyntra.Domain.Dtos.WorkoutSessionDto;

public class WorkoutSessionResponseDto
{
    public long Id { get; set; }
    public string WorkoutDay { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public long? CheckInId { get; set; }
    public List<ExerciseLogResponseDto> ExerciseLogs { get; set; } = [];
}
