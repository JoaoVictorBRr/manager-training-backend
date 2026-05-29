using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class EvolutionPhotoRepository(ZyntraContext zyntraContext) : BaseRepository<EvolutionPhoto>(zyntraContext), IEvolutionPhotoRepository
{
    public async Task<IEnumerable<EvolutionPhoto>> GetByStudentAsync(long studentId)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.StudentId == studentId && p.Situation == Domain.Enum.Situation.Active)
            .OrderByDescending(p => p.TakenAt)
            .ToListAsync();
    }
}
