using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IExerciseRepository : IRepositoryBase<Exercise>
{
    Task<IEnumerable<Exercise>> GetByWorkoutSheetAsync(long workoutSheetId);
}
