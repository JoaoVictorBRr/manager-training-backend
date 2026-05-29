using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class CheckInRepository(ZyntraContext zyntraContext) : BaseRepository<CheckIn>(zyntraContext), ICheckInRepository
{
    public async Task<CheckIn> GetTodayByStudentAsync(long studentId)
    {
        var today = DateTime.Today;
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.StudentId == studentId && c.DateTime.Date == today);
    }

    public async Task<IEnumerable<CheckIn>> GetByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Where(c => c.StudentId == studentId)
            .OrderByDescending(c => c.DateTime)
            .ToListAsync();
    }
}
