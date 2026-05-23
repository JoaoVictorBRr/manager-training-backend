using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class ExerciseService(IExerciseRepository repo, IValidator<Exercise> validator) : IExerciseService
{
    private readonly IExerciseRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Exercise> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Exercise> AddAsync(Exercise entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Exercise> UpdateAsync(Exercise entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Exercise> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Exercise>> AddRangeListAsync(IList<Exercise> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Exercise> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Exercise> DeleteAsync(Exercise entity) => _repo.DeleteAsync(entity);
    public Task<Exercise> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Exercise> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Exercise> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Exercise>> GetAllAsync(Expression<Func<Exercise, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Exercise> GetAsync(Expression<Func<Exercise, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<IEnumerable<Exercise>> GetByWorkoutSheetAsync(long workoutSheetId)
        => _repo.GetByWorkoutSheetAsync(workoutSheetId);

    public async Task<List<ValidationFailure>> Validate(Exercise entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
