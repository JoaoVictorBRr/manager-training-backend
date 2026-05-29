using Zyntra.Domain.Dtos.WorkoutSessionDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IWorkoutSessionService : IServiceBase<WorkoutSession>
{
    Task<WorkoutSession> StartSessionAsync(long studentId, string workoutDay);
    Task<ExerciseLog> LogExerciseAsync(long sessionId, LogExerciseDto dto);
    Task<WorkoutSession> FinishSessionAsync(long sessionId, long? checkInId);
    Task<WorkoutSession?> GetActiveSessionAsync(long studentId);
    Task<IEnumerable<WorkoutSession>> GetCompletedSessionsAsync(long studentId);
    Task<ExerciseLog?> GetLastExerciseLogAsync(long studentId, string exerciseName);
}
