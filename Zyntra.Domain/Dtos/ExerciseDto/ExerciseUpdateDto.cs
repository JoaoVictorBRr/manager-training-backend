namespace Zyntra.Domain.Dtos.ExerciseDto;

public class ExerciseUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string MuscleGroup { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
    public decimal? SuggestedLoad { get; set; }
    public string VideoUrl { get; set; }
    public string Description { get; set; }
}
