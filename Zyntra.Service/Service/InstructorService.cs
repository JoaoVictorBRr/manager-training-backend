using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class InstructorService(IInstructorRepository repo, IValidator<Instructor> validator) : IInstructorService
{
    private readonly IInstructorRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Instructor> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Instructor> AddAsync(Instructor entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Instructor> UpdateAsync(Instructor entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Instructor> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Instructor>> AddRangeListAsync(IList<Instructor> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Instructor> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Instructor> DeleteAsync(Instructor entity) => _repo.DeleteAsync(entity);
    public Task<Instructor> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Instructor> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Instructor> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Instructor>> GetAllAsync(Expression<Func<Instructor, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Instructor> GetAsync(Expression<Func<Instructor, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<Instructor>> FilterAllInstructors(InstructorRequestListDto filter)
        => _repo.FilterAllInstructors(filter);

    public async Task<List<ValidationFailure>> Validate(Instructor entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
