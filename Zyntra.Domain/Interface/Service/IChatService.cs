using Zyntra.Domain.Dtos.ChatDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface IChatService : IServiceBase<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetConversationAsync(long studentId, long instructorId);
    Task<ChatMessage> SendMessageAsync(ChatMessageSendDto dto);
}
