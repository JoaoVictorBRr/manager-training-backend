using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IChatMessageRepository : IRepositoryBase<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetConversationAsync(long studentId, long instructorId);
}
