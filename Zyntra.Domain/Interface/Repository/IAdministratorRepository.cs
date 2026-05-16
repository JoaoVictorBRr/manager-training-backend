using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IAdministratorRepository : IRepositoryBase<Administrator>
{
    Task<Administrator> GetByUserIdAsync(long userId);
}
