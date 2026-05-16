using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.CheckInDto;

public class CheckInPartnerRequestDto
{
    public string PartnerToken { get; set; }
    public CheckInType PartnerType { get; set; }
}
