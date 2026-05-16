using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class PartnerIntegration : EntityBase
{
    public string PartnerName { get; set; }
    public PartnerType IntegrationType { get; set; }
    public string Token { get; set; }
    public CheckInStatus ValidationStatus { get; set; }
}
