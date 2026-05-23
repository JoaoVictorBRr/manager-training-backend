using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class AdministratorService(IAdministratorRepository repo, IValidator<Administrator> validator) : IAdministratorService
{
    private readonly IAdministratorRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Administrator> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Administrator> AddAsync(Administrator entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Administrator> UpdateAsync(Administrator entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Administrator> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Administrator>> AddRangeListAsync(IList<Administrator> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Administrator> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Administrator> DeleteAsync(Administrator entity) => _repo.DeleteAsync(entity);
    public Task<Administrator> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Administrator> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Administrator> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Administrator>> GetAllAsync(Expression<Func<Administrator, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Administrator> GetAsync(Expression<Func<Administrator, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<List<ValidationFailure>> Validate(Administrator entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
