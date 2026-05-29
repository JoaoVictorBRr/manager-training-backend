using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository.Base;

namespace Zyntra.Domain.Interface.Repository;

public interface IAiChatRepository : IRepositoryBase<AiChatMessage>
{
    Task<IEnumerable<AiChatMessage>> GetHistoryAsync(long studentId, int limit = 50);
}
