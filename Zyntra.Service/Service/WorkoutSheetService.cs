using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class WorkoutSheetService(
    IWorkoutSheetRepository workoutSheetRepository,
    IExerciseRepository exerciseRepository) : BaseService<WorkoutSheet>(workoutSheetRepository), IWorkoutSheetService
{
    private readonly IWorkoutSheetRepository _workoutSheetRepository = workoutSheetRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;

    public Task<WorkoutSheet> GetActiveByStudentAsync(long studentId)
        => _workoutSheetRepository.GetActiveByStudentAsync(studentId);

    public async Task<WorkoutSheet> AssignExercisesAsync(long workoutSheetId, IList<Exercise> exercises)
    {
        var sheet = await _workoutSheetRepository.GetByIdAsync(workoutSheetId);
        if (sheet == null)
            throw new InvalidOperationException("Ficha de treino não encontrada.");

        foreach (var exercise in exercises)
            exercise.WorkoutSheetId = workoutSheetId;

        await _exerciseRepository.AddRangeAsync(exercises);
        return await _workoutSheetRepository.GetActiveByStudentAsync(sheet.StudentId);
    }
}
