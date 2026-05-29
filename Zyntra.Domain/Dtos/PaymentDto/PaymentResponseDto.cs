using System.Text.Json.Serialization;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.PaymentDto;

public class PaymentResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    [JsonPropertyName("valor")]
    public decimal Amount { get; set; }
    [JsonPropertyName("dataVencimento")]
    public DateTime DueDate { get; set; }
    [JsonPropertyName("dataPagamento")]
    public DateTime? PaymentDate { get; set; }
    [JsonPropertyName("statusPagamento")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus PaymentStatus { get; set; }
    [JsonPropertyName("metodoPagamento")]
    public string PaymentMethod { get; set; }
}
