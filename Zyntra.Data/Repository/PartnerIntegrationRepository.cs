using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class PartnerIntegrationRepository(ZyntraContext zyntraContext) : BaseRepository<PartnerIntegration>(zyntraContext), IPartnerIntegrationRepository
{
    public async Task<PartnerIntegration> GetByTokenAsync(string token, PartnerType partnerType)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Token == token && p.IntegrationType == partnerType && p.Situation == Situation.Active);
    }
}
