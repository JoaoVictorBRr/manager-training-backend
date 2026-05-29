namespace Zyntra.Domain.Dtos.WorkoutSessionDto;

public class StartWorkoutSessionDto
{
    public long StudentId { get; set; }
    public string WorkoutDay { get; set; } = "A";
}
