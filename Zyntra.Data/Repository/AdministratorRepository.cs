using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class AdministratorRepository(ZyntraContext zyntraContext) : BaseRepository<Administrator>(zyntraContext), IAdministratorRepository
{
    public async Task<Administrator> GetByUserIdAsync(long userId)
    {
        return await DbSet.AsNoTracking()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }
}
