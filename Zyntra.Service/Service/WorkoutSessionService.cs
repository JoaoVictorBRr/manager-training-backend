using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.WorkoutSessionDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class WorkoutSessionService(
    IWorkoutSessionRepository repo,
    IValidator<WorkoutSession> validator) : IWorkoutSessionService
{
    private readonly IWorkoutSessionRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<WorkoutSession> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<WorkoutSession> AddAsync(WorkoutSession entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<WorkoutSession> UpdateAsync(WorkoutSession entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<WorkoutSession> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<WorkoutSession>> AddRangeListAsync(IList<WorkoutSession> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<WorkoutSession> entity) => _repo.UpdateRangeAsync(entity);
    public Task<WorkoutSession> DeleteAsync(WorkoutSession entity) => _repo.DeleteAsync(entity);
    public Task<WorkoutSession> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<WorkoutSession> entities) => _repo.DeleteRangeAsync(entities);
    public Task<WorkoutSession?> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<WorkoutSession>> GetAllAsync(Expression<Func<WorkoutSession, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<WorkoutSession> GetAsync(Expression<Func<WorkoutSession, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<WorkoutSession> StartSessionAsync(long studentId, string workoutDay)
    {
        var session = new WorkoutSession
        {
            StudentId = studentId,
            WorkoutDay = workoutDay,
            StartedAt = DateTime.Now,
        };
        return await AddAsync(session);
    }

    public async Task<ExerciseLog> LogExerciseAsync(long sessionId, LogExerciseDto dto)
    {
        var exists = await _repo.GetByIdAsync(sessionId);
        if (exists == null) throw new InvalidOperationException("Sessão de treino não encontrada.");

        var log = new ExerciseLog
        {
            WorkoutSessionId = sessionId,
            ExerciseId = dto.ExerciseId,
            ExerciseName = dto.ExerciseName,
            SetsCompleted = dto.SetsCompleted,
            RepsJson = dto.RepsJson,
            WeightsJson = dto.WeightsJson,
            DurationSeconds = dto.DurationSeconds,
            DistanceMeters = dto.DistanceMeters,
            Notes = dto.Notes,
            CompletedAt = DateTime.Now,
        };

        return await _repo.AddExerciseLogAsync(log);
    }

    public async Task<WorkoutSession> FinishSessionAsync(long sessionId, long? checkInId)
    {
        var session = await _repo.GetByIdAsync(sessionId)
            ?? throw new InvalidOperationException("Sessão de treino não encontrada.");

        session.FinishedAt = DateTime.Now;
        session.CheckInId = checkInId;
        return await _repo.UpdateAsync(session);
    }

    public Task<WorkoutSession?> GetActiveSessionAsync(long studentId)
        => _repo.GetActiveByStudentAsync(studentId);

    public Task<IEnumerable<WorkoutSession>> GetCompletedSessionsAsync(long studentId)
        => _repo.GetCompletedByStudentAsync(studentId);

    public Task<ExerciseLog?> GetLastExerciseLogAsync(long studentId, string exerciseName)
        => _repo.GetLastExerciseLogByNameAsync(studentId, exerciseName);

    public async Task<List<ValidationFailure>> Validate(WorkoutSession entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
