using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IPartnerIntegrationRepository : IRepositoryBase<PartnerIntegration>
{
    Task<PartnerIntegration> GetByTokenAsync(string token, PartnerType partnerType);
}
