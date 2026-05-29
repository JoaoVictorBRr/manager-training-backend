using System.Text.Json.Serialization;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ExerciseDto;

public class ExerciseResponseDto
{
    public long Id { get; set; }
    public long WorkoutSheetId { get; set; }
    [JsonPropertyName("nome")]
    public string Name { get; set; }
    [JsonPropertyName("grupoMuscular")]
    public string MuscleGroup { get; set; }
    [JsonPropertyName("series")]
    public int Sets { get; set; }
    [JsonPropertyName("repeticoes")]
    public string Repetitions { get; set; }
    [JsonPropertyName("cargaSugerida")]
    public decimal? SuggestedLoad { get; set; }
    [JsonPropertyName("urlVideo")]
    public string VideoUrl { get; set; }
    [JsonPropertyName("descricao")]
    public string Description { get; set; }
    [JsonPropertyName("descanso")]
    public string? RestTime { get; set; }
    [JsonPropertyName("dropset")]
    public bool IsDropset { get; set; }
    [JsonPropertyName("superset")]
    public string? SupersetWith { get; set; }
    [JsonPropertyName("ateAFalha")]
    public bool ToFailure { get; set; }
    [JsonPropertyName("rir")]
    public int? RIR { get; set; }
    [JsonPropertyName("cadencia")]
    public string? Cadence { get; set; }
    [JsonPropertyName("tecnicas")]
    public string? AdvancedTechniques { get; set; }
    [JsonPropertyName("dificuldade")]
    public string? Difficulty { get; set; }
    [JsonPropertyName("caloriasEstimadas")]
    public int? EstimatedCalories { get; set; }
    public string WorkoutDay { get; set; } = "A";
    public ExerciseType ExerciseType { get; set; } = ExerciseType.WeightTraining;
}
