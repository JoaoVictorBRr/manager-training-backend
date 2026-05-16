using Zyntra.Domain.Entities.Base;
using Zyntra.Domain.Enum;

namespace Zyntra.Domain.Entities;

public class Notification : EntityBase
{
    public long UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime SendDateTime { get; set; }
    public bool IsRead { get; set; }

    public User User { get; set; }
}
