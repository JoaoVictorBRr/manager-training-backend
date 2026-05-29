using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class AiChatRepository(ZyntraContext zyntraContext) : BaseRepository<AiChatMessage>(zyntraContext), IAiChatRepository
{
    public async Task<IEnumerable<AiChatMessage>> GetHistoryAsync(long studentId, int limit = 50)
    {
        return await DbSet.AsNoTracking()
            .Where(m => m.StudentId == studentId)
            .OrderBy(m => m.DateCreated)
            .Take(limit)
            .ToListAsync();
    }
}
