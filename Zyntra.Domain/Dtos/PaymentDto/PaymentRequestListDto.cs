using Zyntra.Domain.Dtos.Base.List;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.PaymentDto;

public class PaymentRequestListDto : BaseRequestListDto
{
    public long? StudentId { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}
