using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IWorkoutSheetService : IServiceBase<WorkoutSheet>
{
    Task<WorkoutSheet> GetActiveByStudentAsync(long studentId);
    Task<WorkoutSheet> AssignExercisesAsync(long workoutSheetId, IList<Exercise> exercises);
}
