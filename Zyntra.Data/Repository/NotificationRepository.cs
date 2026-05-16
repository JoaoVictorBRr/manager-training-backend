using Microsoft.EntityFrameworkCore;
using Zyntra.Data.Context;
using Zyntra.Data.Repository.Base;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Interface.Repository;

namespace Zyntra.Data.Repository;

public class NotificationRepository(ZyntraContext zyntraContext) : BaseRepository<Notification>(zyntraContext), INotificationRepository
{
    public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(long userId)
    {
        return await DbSet.AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.SendDateTime)
            .ToListAsync();
    }
}
