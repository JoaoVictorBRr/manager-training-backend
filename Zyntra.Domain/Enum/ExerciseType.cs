using System.ComponentModel;

namespace Zyntra.Domain.Enum;

public enum ExerciseType
{
    [Description("Musculação")]
    WeightTraining = 1,
    [Description("Cardio")]
    Cardio = 2,
    [Description("Peso Corporal")]
    Bodyweight = 3,
}
