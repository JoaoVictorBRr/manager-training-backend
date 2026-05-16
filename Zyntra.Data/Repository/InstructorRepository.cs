using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class InstructorRepository(ZyntraContext zyntraContext) : BaseRepository<Instructor>(zyntraContext), IInstructorRepository
{
    public async Task<PagedListDto<Instructor>> FilterAllInstructors(InstructorRequestListDto filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(i => i.User)
            .Where(i => filter.Situation == Situation.ForFilterActiveAndInactive || i.Situation == filter.Situation);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(i => i.User.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Specialty))
            query = query.Where(i => i.Specialty.Contains(filter.Specialty));

        var total = await query.CountAsync();
        var items = await query.Skip(filter.Start).Take(filter.Take).ToListAsync();
        return new PagedListDto<Instructor>(items, filter.Start, filter.Take, total);
    }

    public async Task<Instructor> GetByUserIdAsync(long userId)
    {
        return await DbSet.AsNoTracking()
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.UserId == userId);
    }
}
