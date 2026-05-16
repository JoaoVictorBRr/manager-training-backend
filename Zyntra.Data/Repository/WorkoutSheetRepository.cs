using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class WorkoutSheetRepository(ZyntraContext zyntraContext) : BaseRepository<WorkoutSheet>(zyntraContext), IWorkoutSheetRepository
{
    public async Task<WorkoutSheet> GetActiveByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Include(w => w.Exercises)
            .Include(w => w.Instructor).ThenInclude(i => i.User)
            .FirstOrDefaultAsync(w => w.StudentId == studentId && w.IsActive);
    }
}
