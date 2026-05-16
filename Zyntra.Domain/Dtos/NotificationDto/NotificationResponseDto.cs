using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Dtos.NotificationDto;

public class NotificationResponseDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime SendDateTime { get; set; }
    public bool IsRead { get; set; }
}
