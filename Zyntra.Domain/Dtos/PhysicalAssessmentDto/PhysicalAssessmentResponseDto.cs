using System.Text.Json.Serialization;

namespace Zyntra.Domain.Dtos.PhysicalAssessmentDto;

public class PhysicalAssessmentResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    [JsonPropertyName("data")]
    public DateTime AssessmentDate { get; set; }
    [JsonPropertyName("peso")]
    public decimal Weight { get; set; }
    [JsonPropertyName("altura")]
    public decimal Height { get; set; }
    [JsonPropertyName("imc")]
    public decimal Bmi { get; set; }
    [JsonPropertyName("percentualGordura")]
    public decimal? BodyFatPercentage { get; set; }
    [JsonPropertyName("medidas")]
    public string Measurements { get; set; }
    [JsonPropertyName("observacoes")]
    public string Notes { get; set; }
}
