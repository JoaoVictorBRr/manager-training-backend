using FluentValidation.Results;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.UserDto.List;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IUserService : IServiceBase<User>
{
    Task<PagedListDto<User>> FilterAllUsers(UseRequestListDto userFilter);
    Task<List<ValidationFailure>> Validate(User entity);
    Task<User> UpdatePasswordAsync(User entity);
}
