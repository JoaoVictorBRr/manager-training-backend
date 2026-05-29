using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class HydrationLogRepository(ZyntraContext zyntraContext) : BaseRepository<HydrationLog>(zyntraContext), IHydrationLogRepository
{
    public async Task<IEnumerable<HydrationLog>> GetTodayLogsAsync(long studentId)
    {
        var today = DateTime.Today;
        return await DbSet.AsNoTracking()
            .Where(h => h.StudentId == studentId && h.LogDate.Date == today)
            .OrderBy(h => h.DateCreated)
            .ToListAsync();
    }
}
