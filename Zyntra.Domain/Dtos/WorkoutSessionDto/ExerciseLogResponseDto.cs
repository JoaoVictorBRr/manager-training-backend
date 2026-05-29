namespace Zyntra.Domain.Dtos.WorkoutSessionDto;

public class ExerciseLogResponseDto
{
    public long Id { get; set; }
    public long ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int SetsCompleted { get; set; }
    public string? RepsJson { get; set; }
    public string? WeightsJson { get; set; }
    public int? DurationSeconds { get; set; }
    public int? DistanceMeters { get; set; }
    public string? Notes { get; set; }
    public DateTime CompletedAt { get; set; }
}
