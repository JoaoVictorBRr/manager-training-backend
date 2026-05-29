using Zyntra.Domain.Dtos.HydrationDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IHydrationService : IServiceBase<HydrationLog>
{
    Task<HydrationSummaryDto> GetTodaySummaryAsync(long studentId, decimal goalMl);
    Task AddLogAsync(long studentId, decimal amountMl);
}
