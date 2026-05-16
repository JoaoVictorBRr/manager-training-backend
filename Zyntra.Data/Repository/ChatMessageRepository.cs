using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class ChatMessageRepository(ZyntraContext zyntraContext) : BaseRepository<ChatMessage>(zyntraContext), IChatMessageRepository
{
    public async Task<IEnumerable<ChatMessage>> GetConversationAsync(long studentId, long instructorId)
    {
        return await DbSet.AsNoTracking()
            .Where(c => c.StudentId == studentId && c.InstructorId == instructorId)
            .OrderBy(c => c.MessageDateTime)
            .ToListAsync();
    }
}
