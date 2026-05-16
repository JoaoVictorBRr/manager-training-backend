using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class ScheduleRepository(ZyntraContext zyntraContext) : BaseRepository<Schedule>(zyntraContext), IScheduleRepository
{
    public async Task<PagedListDto<Schedule>> FilterAllSchedules(ScheduleRequestListDto filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(s => s.Student).ThenInclude(st => st.User)
            .Include(s => s.Class)
            .Where(s => filter.Situation == Situation.ForFilterActiveAndInactive || s.Situation == filter.Situation);

        if (filter.StudentId.HasValue)
            query = query.Where(s => s.StudentId == filter.StudentId.Value);

        if (filter.ClassId.HasValue)
            query = query.Where(s => s.ClassId == filter.ClassId.Value);

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(s => s.ReservationDate).Skip(filter.Start).Take(filter.Take).ToListAsync();
        return new PagedListDto<Schedule>(items, filter.Start, filter.Take, total);
    }

    public async Task<Schedule> GetByStudentAndClassAsync(long studentId, long classId)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.ClassId == classId && s.Situation == Situation.Active);
    }

    public async Task<IEnumerable<Schedule>> GetActiveByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Include(s => s.Class).ThenInclude(c => c.Instructor).ThenInclude(i => i.User)
            .Where(s => s.StudentId == studentId && s.Status == ScheduleStatus.Confirmed && s.Situation == Situation.Active)
            .OrderBy(s => s.Class.DateTime)
            .ToListAsync();
    }
}
