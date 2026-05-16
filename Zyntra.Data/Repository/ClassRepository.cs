using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class ClassRepository(ZyntraContext zyntraContext) : BaseRepository<Class>(zyntraContext), IClassRepository
{
    public async Task<PagedListDto<Class>> FilterAllClasses(ClassRequestListDto filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(c => c.Instructor).ThenInclude(i => i.User)
            .Where(c => filter.Situation == Situation.ForFilterActiveAndInactive || c.Situation == filter.Situation);

        if (!string.IsNullOrWhiteSpace(filter.Modality))
            query = query.Where(c => c.Modality.Contains(filter.Modality));

        if (!string.IsNullOrWhiteSpace(filter.Unit))
            query = query.Where(c => c.Unit.Contains(filter.Unit));

        if (filter.DateFrom.HasValue)
            query = query.Where(c => c.DateTime >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(c => c.DateTime <= filter.DateTo.Value);

        if (filter.OnlyAvailable == true)
            query = query.Where(c => c.AvailableSlots > 0);

        var total = await query.CountAsync();
        var items = await query.OrderBy(c => c.DateTime).Skip(filter.Start).Take(filter.Take).ToListAsync();
        return new PagedListDto<Class>(items, filter.Start, filter.Take, total);
    }

    public async Task<IEnumerable<Class>> GetAvailableClassesAsync()
    {
        return await DbSet.AsNoTracking()
            .Include(c => c.Instructor).ThenInclude(i => i.User)
            .Where(c => c.AvailableSlots > 0 && c.Situation == Situation.Active && c.DateTime > DateTime.Now)
            .OrderBy(c => c.DateTime)
            .ToListAsync();
    }
}
