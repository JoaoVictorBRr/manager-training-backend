using FluentValidation.Results;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.UserDto.List;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<PagedListDto<User>> FilterAllUsers(UseRequestListDto userFilter);
}
