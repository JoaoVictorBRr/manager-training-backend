using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Service.Base;

namespace Zyntra.Domain.Interface.Service;

public interface INotificationService : IServiceBase<Notification>
{
    Task<IEnumerable<Notification>> GetByUserAsync(long userId);
    Task MarkAsReadAsync(long notificationId);
    Task SendNotificationAsync(long userId, NotificationType type, string title, string message);
}
