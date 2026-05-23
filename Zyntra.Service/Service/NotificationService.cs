using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;
using Zyntra.Domain.Interface.Repository;
using Zyntra.Domain.Interface.Service;

namespace Zyntra.Service.Service;

public class NotificationService(INotificationRepository repo, IValidator<Notification> validator) : INotificationService
{
    private readonly INotificationRepository _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    private readonly IValidator<Notification> _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task<Notification> AddAsync(Notification entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.AddAsync(entity);
    }

    public async Task<Notification> UpdateAsync(Notification entity)
    {
        _ = entity ?? throw new ArgumentNullException(nameof(entity));
        var errors = await Validate(entity);
        if (errors is { } && errors.Any()) throw new ValidationException(errors);
        return await _repo.UpdateAsync(entity);
    }

    public Task AddRangeAsync(IList<Notification> entity) => _repo.AddRangeAsync(entity);
    public Task<IList<Notification>> AddRangeListAsync(IList<Notification> entities) => _repo.AddRangeListAsync(entities);
    public Task UpdateRangeAsync(IEnumerable<Notification> entity) => _repo.UpdateRangeAsync(entity);
    public Task<Notification> DeleteAsync(Notification entity) => _repo.DeleteAsync(entity);
    public Task<Notification> DeleteAsync(long Id) => _repo.DeleteAsync(Id);
    public Task DeleteRangeAsync(IList<Notification> entities) => _repo.DeleteRangeAsync(entities);
    public Task<Notification> GetByIdAsync(long Id) => _repo.GetByIdAsync(Id);
    public Task<IEnumerable<Notification>> GetAllAsync(Expression<Func<Notification, bool>> predicate) => _repo.GetAllAsync(predicate);
    public Task<Notification> GetAsync(Expression<Func<Notification, bool>> predicate) => _repo.GetAsync(predicate);

    public async Task<IEnumerable<Notification>> GetByUserAsync(long userId)
        => await _repo.GetAllAsync(n => n.UserId == userId);

    public async Task MarkAsReadAsync(long notificationId)
    {
        var notification = await _repo.GetByIdAsync(notificationId);
        if (notification == null) return;

        notification.IsRead = true;
        await this.UpdateAsync(notification);
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

        await this.AddAsync(notification);
    }

    public async Task<List<ValidationFailure>> Validate(Notification entity)
    {
        var errors = new List<ValidationFailure>();
        var validation = _validator.Validate(entity);
        if (!validation.IsValid) errors.AddRange(validation.Errors);
        return await Task.FromResult(errors);
    }
}
