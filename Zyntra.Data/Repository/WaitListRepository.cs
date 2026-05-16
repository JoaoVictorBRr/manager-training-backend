using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class WaitListRepository(ZyntraContext zyntraContext) : BaseRepository<WaitList>(zyntraContext), IWaitListRepository
{
    public async Task<WaitList> GetFirstInLineAsync(long classId)
    {
        return await DbSet.AsNoTracking()
            .Where(w => w.ClassId == classId && w.Situation == Situation.Active)
            .OrderBy(w => w.Position)
            .FirstOrDefaultAsync();
    }

    public async Task<WaitList> GetByStudentAndClassAsync(long studentId, long classId)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(w => w.StudentId == studentId && w.ClassId == classId && w.Situation == Situation.Active);
    }
}
