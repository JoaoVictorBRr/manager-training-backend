using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Dtos.Pagination;
using Zyntra.Domain.Dtos.UserDto.List;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class UserRepository(ZyntraContext zyntraContext) : BaseRepository<User>(zyntraContext), IUserRepository
{
    public Task<PagedListDto<User>> FilterAllUsers(UseRequestListDto userFilter)
    {
        throw new NotImplementedException();
    }
}
