using Zyntra.Domain.Dtos.ChatDto;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class ChatService(IChatMessageRepository chatMessageRepository) : BaseService<ChatMessage>(chatMessageRepository), IChatService
{
    private readonly IChatMessageRepository _chatMessageRepository = chatMessageRepository;

    public Task<IEnumerable<ChatMessage>> GetConversationAsync(long studentId, long instructorId)
        => _chatMessageRepository.GetConversationAsync(studentId, instructorId);

    public async Task<ChatMessage> SendMessageAsync(ChatMessageSendDto dto)
    {
        var message = new ChatMessage
        {
            StudentId = dto.StudentId,
            InstructorId = dto.InstructorId,
            Message = dto.Message,
            MessageDateTime = DateTime.Now,
            IsRead = false
        };

        return await _chatMessageRepository.AddAsync(message);
    }
}
