using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class StudentDietRepository(ZyntraContext zyntraContext) : BaseRepository<StudentDiet>(zyntraContext), IStudentDietRepository
{
    public async Task<StudentDiet?> GetActiveDietByStudentAsync(long studentId)
    {
        return await DbSet
            .AsNoTracking()
            .Include(d => d.Meals.Where(m => m.Situation == Domain.Enum.Situation.Active))
                .ThenInclude(m => m.Options.Where(o => o.Situation == Domain.Enum.Situation.Active))
                    .ThenInclude(o => o.Photos.Where(p => p.Situation == Domain.Enum.Situation.Active))
            .Where(d => d.StudentId == studentId && d.IsActive && d.Situation == Domain.Enum.Situation.Active)
            .OrderByDescending(d => d.DateCreated)
            .FirstOrDefaultAsync();
    }
}
