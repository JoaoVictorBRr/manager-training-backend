using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class WorkoutSessionRepository(ZyntraContext zyntraContext) : BaseRepository<WorkoutSession>(zyntraContext), IWorkoutSessionRepository
{
    public async Task<WorkoutSession?> GetActiveByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Include(s => s.ExerciseLogs)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.FinishedAt == null);
    }

    public async Task<IEnumerable<WorkoutSession>> GetCompletedByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Include(s => s.ExerciseLogs)
            .Where(s => s.StudentId == studentId && s.FinishedAt != null)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }

    public async Task<ExerciseLog> AddExerciseLogAsync(ExerciseLog log)
    {
        log.DateCreated = DateTime.Now;
        await zyntraContext.Set<ExerciseLog>().AddAsync(log);
        await zyntraContext.SaveChangesAsync();
        return log;
    }

    public async Task<ExerciseLog?> GetLastExerciseLogByNameAsync(long studentId, string exerciseName)
    {
        return await zyntraContext.Set<ExerciseLog>()
            .AsNoTracking()
            .Include(l => l.WorkoutSession)
            .Where(l => l.WorkoutSession.StudentId == studentId
                     && l.ExerciseName == exerciseName
                     && l.WorkoutSession.FinishedAt != null)
            .OrderByDescending(l => l.CompletedAt)
            .FirstOrDefaultAsync();
    }
}
