using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.PaymentDto;

public class PaymentResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }
}
