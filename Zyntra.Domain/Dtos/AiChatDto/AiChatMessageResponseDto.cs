namespace Zyntra.Domain.Dtos.AiChatDto;

public class AiChatMessageResponseDto
{
    public long Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AiChatActionDto? Action { get; set; }
    public string ActionStatus { get; set; } = "none";
    public DateTime CreatedAt { get; set; }
}
