using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class ClassService(IClassRepository repo, IValidator<Class> validator) : IClassService
{
    private readonly IClassRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Class> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Class> AddAsync(Class entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Class> UpdateAsync(Class entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Class> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Class>> AddRangeListAsync(IList<Class> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Class> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Class> DeleteAsync(Class entity) => _repo.DeleteAsync(entity);
    public Task<Class> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Class> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Class> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Class>> GetAllAsync(Expression<Func<Class, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Class> GetAsync(Expression<Func<Class, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<Class>> FilterAllClasses(ClassRequestListDto filter)
        => _repo.FilterAllClasses(filter);

    public Task<IEnumerable<Class>> GetAvailableClassesAsync()
        => _repo.GetAvailableClassesAsync();

    public async Task<List<ValidationFailure>> Validate(Class entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
