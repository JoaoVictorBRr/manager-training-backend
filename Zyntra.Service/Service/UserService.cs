using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.UserDto.List;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    public Task<User> AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IList<User> entity)
    {
        throw new NotImplementedException();
    }

    public Task<IList<User>> AddRangeListAsync(IList<User> entities)
    {
        throw new NotImplementedException();
    }

    public Task<User> DeleteAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User> DeleteAsync(long Id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IList<User> entities)
    {
        throw new NotImplementedException();
    }

    public Task<PagedListDto<User>> FilterAllUsers(UseRequestListDto userFilter)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetAsync(Expression<Func<User, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(long Id)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdatePasswordAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<User> entity)
    {
        throw new NotImplementedException();
    }

    public Task<List<ValidationFailure>> Validate(User entity)
    {
        throw new NotImplementedException();
    }
}
