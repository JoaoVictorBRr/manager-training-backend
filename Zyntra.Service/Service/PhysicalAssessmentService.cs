using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class PhysicalAssessmentService(IPhysicalAssessmentRepository repo, IValidator<PhysicalAssessment> validator) : IPhysicalAssessmentService
{
    private readonly IPhysicalAssessmentRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<PhysicalAssessment> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<PhysicalAssessment> AddAsync(PhysicalAssessment entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        entity.Bmi = Math.Round(entity.Weight / (entity.Height * entity.Height), 2);
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<PhysicalAssessment> UpdateAsync(PhysicalAssessment entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<PhysicalAssessment> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<PhysicalAssessment>> AddRangeListAsync(IList<PhysicalAssessment> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<PhysicalAssessment> entity) => _repo.UpdateRangeAsync(entity);
    public Task<PhysicalAssessment> DeleteAsync(PhysicalAssessment entity) => _repo.DeleteAsync(entity);
    public Task<PhysicalAssessment> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<PhysicalAssessment> entities) => _repo.DeleteRangeAsync(entities);
    public Task<PhysicalAssessment> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<PhysicalAssessment>> GetAllAsync(Expression<Func<PhysicalAssessment, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<PhysicalAssessment> GetAsync(Expression<Func<PhysicalAssessment, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<IEnumerable<PhysicalAssessment>> GetHistoryByStudentAsync(long studentId)
        => _repo.GetHistoryByStudentAsync(studentId);

    public async Task<List<ValidationFailure>> Validate(PhysicalAssessment entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
