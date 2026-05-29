namespace Zyntra.Domain.Dtos.ExerciseDto;

public class ExerciseUpdateDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string MuscleGroup { get; set; }
    public int Sets { get; set; }
    public string Repetitions { get; set; }
    public decimal? SuggestedLoad { get; set; }
    public string VideoUrl { get; set; }
    public string Description { get; set; }
    public string? RestTime { get; set; }
    public bool IsDropset { get; set; }
    public string? SupersetWith { get; set; }
    public bool ToFailure { get; set; }
    public int? RIR { get; set; }
    public string? Cadence { get; set; }
    public string? AdvancedTechniques { get; set; }
    public string? Difficulty { get; set; }
    public int? EstimatedCalories { get; set; }
}
