using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IWorkoutSessionRepository : IRepositoryBase<WorkoutSession>
{
    Task<WorkoutSession?> GetActiveByStudentAsync(long studentId);
    Task<IEnumerable<WorkoutSession>> GetCompletedByStudentAsync(long studentId);
    Task<ExerciseLog> AddExerciseLogAsync(ExerciseLog log);
    Task<ExerciseLog?> GetLastExerciseLogByNameAsync(long studentId, string exerciseName);
}
