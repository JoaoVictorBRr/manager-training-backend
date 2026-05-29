using Zyntra.Domain.Dtos.AiChatDto;

namespace Zyntra.Domain.Interface.Service;

public interface IAiChatService
{
    Task<AiChatMessageResponseDto> SendMessageAsync(long userId, long studentId, string userMessage);
    Task<IEnumerable<AiChatMessageResponseDto>> GetHistoryAsync(long studentId);
    Task ConfirmActionAsync(long messageId, long studentId);
    Task RejectActionAsync(long messageId, long studentId);
}
