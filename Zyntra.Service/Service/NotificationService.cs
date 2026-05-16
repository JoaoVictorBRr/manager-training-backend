using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;
using Zyntra.Service.Service.Base;

namespace Zyntra.Service.Service;

public class NotificationService(INotificationRepository notificationRepository) : BaseService<Notification>(notificationRepository), INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;

    public async Task<IEnumerable<Notification>> GetByUserAsync(long userId)
    {
        return await _notificationRepository.GetAllAsync(n => n.UserId == userId);
    }

    public async Task MarkAsReadAsync(long notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null) return;

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
    }

    public async Task SendNotificationAsync(long userId, NotificationType type, string title, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            SendDateTime = DateTime.Now,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);
    }
}
