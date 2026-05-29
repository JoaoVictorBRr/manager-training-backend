using System.Text.Json.Serialization;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ClassDto;

public class ClassResponseDto
{
    public long Id { get; set; }
    [JsonPropertyName("modalidade")]
    public string Modality { get; set; }
    [JsonPropertyName("dataHora")]
    public DateTime DateTime { get; set; }
    [JsonPropertyName("capacidadeMaxima")]
    public int MaxCapacity { get; set; }
    [JsonPropertyName("vagasDisponiveis")]
    public int AvailableSlots { get; set; }
    [JsonPropertyName("unidade")]
    public string Unit { get; set; }
    public long InstructorId { get; set; }
    [JsonPropertyName("instructorNome")]
    public string InstructorName { get; set; }
    public Situation Situation { get; set; }
}
