using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class WaitListService(IWaitListRepository repo, IValidator<WaitList> validator) : IWaitListService
{
    private readonly IWaitListRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<WaitList> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<WaitList> AddAsync(WaitList entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<WaitList> UpdateAsync(WaitList entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<WaitList> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<WaitList>> AddRangeListAsync(IList<WaitList> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<WaitList> entity) => _repo.UpdateRangeAsync(entity);
    public Task<WaitList> DeleteAsync(WaitList entity) => _repo.DeleteAsync(entity);
    public Task<WaitList> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<WaitList> entities) => _repo.DeleteRangeAsync(entities);
    public Task<WaitList> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<WaitList>> GetAllAsync(Expression<Func<WaitList, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<WaitList> GetAsync(Expression<Func<WaitList, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<List<ValidationFailure>> Validate(WaitList entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
