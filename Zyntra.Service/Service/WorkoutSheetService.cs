using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class WorkoutSheetService(
    IWorkoutSheetRepository repo,
    IValidator<WorkoutSheet> validator,
    IExerciseRepository exerciseRepository) : IWorkoutSheetService
{
    private readonly IWorkoutSheetRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<WorkoutSheet> _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));

    public async Task<WorkoutSheet> AddAsync(WorkoutSheet entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<WorkoutSheet> UpdateAsync(WorkoutSheet entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<WorkoutSheet> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<WorkoutSheet>> AddRangeListAsync(IList<WorkoutSheet> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<WorkoutSheet> entity) => _repo.UpdateRangeAsync(entity);
    public Task<WorkoutSheet> DeleteAsync(WorkoutSheet entity) => _repo.DeleteAsync(entity);
    public Task<WorkoutSheet> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<WorkoutSheet> entities) => _repo.DeleteRangeAsync(entities);
    public Task<WorkoutSheet> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<WorkoutSheet>> GetAllAsync(Expression<Func<WorkoutSheet, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<WorkoutSheet> GetAsync(Expression<Func<WorkoutSheet, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<WorkoutSheet> GetActiveByStudentAsync(long studentId)
        => _repo.GetActiveByStudentAsync(studentId);

    public async Task<WorkoutSheet> AssignExercisesAsync(long workoutSheetId, IList<Exercise> exercises)
    {
        var sheet = await _repo.GetByIdAsync(workoutSheetId);
        if (sheet == null)
            throw new InvalidOperationException("Ficha de treino não encontrada.");

        foreach (var exercise in exercises)
            exercise.WorkoutSheetId = workoutSheetId;

        await _exerciseRepository.AddRangeAsync(exercises);
        return await _repo.GetActiveByStudentAsync(sheet.StudentId);
    }

    public async Task<List<ValidationFailure>> Validate(WorkoutSheet entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
