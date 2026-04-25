using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Application.Repositories;

public interface INotificationRepository
{
    Task AddAsync(NotificationEntity notification, CancellationToken ct = default);
    Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<NotificationEntity>> GetByUserIdAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<NotificationEntity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default);
    Task<int> CountAllAsync(CancellationToken ct = default);
    Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct = default);
    Task UpdateAsync(NotificationEntity notification, CancellationToken ct = default);
}
