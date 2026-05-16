using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class StudentRepository(ZyntraContext zyntraContext) : BaseRepository<Student>(zyntraContext), IStudentRepository
{
    public async Task<PagedListDto<Student>> FilterAllStudents(StudentRequestListDto filter)
    {
        var query = DbSet.AsNoTracking()
            .Include(s => s.User)
            .Where(s => filter.Situation == Situation.ForFilterActiveAndInactive || s.Situation == filter.Situation);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(s => s.User.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(s => s.User.Email.Contains(filter.Email));

        var total = await query.CountAsync();
        var items = await query.Skip(filter.Start).Take(filter.Take).ToListAsync();
        return new PagedListDto<Student>(items, filter.Start, filter.Take, total);
    }

    public async Task<Student> GetByUserIdAsync(long userId)
    {
        return await DbSet.AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
