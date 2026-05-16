using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class ExerciseRepository(ZyntraContext zyntraContext) : BaseRepository<Exercise>(zyntraContext), IExerciseRepository
{
    public async Task<IEnumerable<Exercise>> GetByWorkoutSheetAsync(long workoutSheetId)
    {
        return await DbSet.AsNoTracking()
            .Where(e => e.WorkoutSheetId == workoutSheetId)
            .ToListAsync();
    }
}
