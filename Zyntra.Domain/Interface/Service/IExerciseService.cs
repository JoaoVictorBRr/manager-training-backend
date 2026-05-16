using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IExerciseService : IServiceBase<Exercise>
{
    Task<IEnumerable<Exercise>> GetByWorkoutSheetAsync(long workoutSheetId);
}
