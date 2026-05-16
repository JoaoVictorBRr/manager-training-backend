namespace Zyntra.Domain.Dtos.ChatDto;

public class ChatMessageSendDto
{
    public long StudentId { get; set; }
    public long InstructorId { get; set; }
    public string Message { get; set; }
}
