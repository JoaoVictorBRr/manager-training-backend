using System.Text.Json.Serialization;
using Zyntra.Domain.Dtos.ExerciseDto;

namespace Zyntra.Domain.Dtos.WorkoutSheetDto;

public class WorkoutSheetResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public long InstructorId { get; set; }
    [JsonPropertyName("instructorNome")]
    public string InstructorName { get; set; }
    [JsonPropertyName("dataInicio")]
    public DateTime StartDate { get; set; }
    [JsonPropertyName("dataFim")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("statusAtivo")]
    public bool IsActive { get; set; }
    [JsonPropertyName("observacoes")]
    public string Notes { get; set; }
    public IEnumerable<ExerciseResponseDto> Exercises { get; set; }
}
