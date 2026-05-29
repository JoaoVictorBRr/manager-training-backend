using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IHydrationLogRepository : IRepositoryBase<HydrationLog>
{
    Task<IEnumerable<HydrationLog>> GetTodayLogsAsync(long studentId);
}
