namespace Zyntra.Domain.Dtos.ChatDto;

public class ChatMessageResponseDto
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; }
    public string Message { get; set; }
    public DateTime MessageDateTime { get; set; }
    public bool IsRead { get; set; }
}
