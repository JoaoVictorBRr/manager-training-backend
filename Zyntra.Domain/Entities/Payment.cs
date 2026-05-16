using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class Payment : EntityBase
{
    public long StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }

    public Student Student { get; set; }
}
