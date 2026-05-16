using Zyntra.Domain.Entities.Base;

namespace Zyntra.Domain.Entities;

public class Exercise : EntityBase
{
    public long WorkoutSheetId { get; set; }
    public string Name { get; set; }
    public string MuscleGroup { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
    public decimal? SuggestedLoad { get; set; }
    public string VideoUrl { get; set; }
    public string Description { get; set; }

    public WorkoutSheet WorkoutSheet { get; set; }
}
