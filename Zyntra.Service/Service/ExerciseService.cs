using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class ExerciseService(IExerciseRepository exerciseRepository) : BaseService<Exercise>(exerciseRepository), IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;

    public Task<IEnumerable<Exercise>> GetByWorkoutSheetAsync(long workoutSheetId)
        => _exerciseRepository.GetByWorkoutSheetAsync(workoutSheetId);
}
