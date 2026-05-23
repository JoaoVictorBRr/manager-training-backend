using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.UserDto.List;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class UserService(IUserRepository repo, IValidator<User> validator) : IUserService
{
    private readonly IUserRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<User> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<User> AddAsync(User entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<User> UpdateAsync(User entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<User> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<User>> AddRangeListAsync(IList<User> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<User> entity) => _repo.UpdateRangeAsync(entity);
    public Task<User> DeleteAsync(User entity) => _repo.DeleteAsync(entity);
    public Task<User> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<User> entities) => _repo.DeleteRangeAsync(entities);
    public Task<User> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<User> GetAsync(Expression<Func<User, bool>> predicate) => _repo.GetAsync(predicate);

    public Task<PagedListDto<User>> FilterAllUsers(UseRequestListDto userFilter)
        => _repo.FilterAllUsers(userFilter);

    public Task<User> UpdatePasswordAsync(User entity)
        => _repo.UpdateAsync(entity);

    public async Task<List<ValidationFailure>> Validate(User entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
