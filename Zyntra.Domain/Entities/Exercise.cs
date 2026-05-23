using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Exercise : EntityBase
{
    public long WorkoutSheetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public int Sets { get; set; }
    public string Repetitions { get; set; } = string.Empty;
    public decimal? SuggestedLoad { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RestTime { get; set; }
    public bool IsDropset { get; set; } = false;
    public string? SupersetWith { get; set; }
    public bool ToFailure { get; set; } = false;
    public int? RIR { get; set; }
    public string? Cadence { get; set; }
    public string? AdvancedTechniques { get; set; }

    public WorkoutSheet WorkoutSheet { get; set; }
}
