using System.Text.Json.Serialization;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.ScheduleDto;

public class ScheduleResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public long ClassId { get; set; }
    [JsonPropertyName("classModalidade")]
    public string ClassModality { get; set; }
    [JsonPropertyName("classDataHora")]
    public DateTime ClassDateTime { get; set; }
    [JsonPropertyName("classUnidade")]
    public string ClassUnit { get; set; }
    [JsonPropertyName("instructorNome")]
    public string InstructorName { get; set; }
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScheduleStatus Status { get; set; }
    [JsonPropertyName("dataReserva")]
    public DateTime ReservationDate { get; set; }
}
