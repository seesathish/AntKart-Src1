using AK.Notification.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(NotificationsDbContext dbContext) : INotificationRepository
{
    public async Task AddAsync(NotificationEntity notification, CancellationToken ct = default)
    {
        await dbContext.Notifications.AddAsync(notification, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<NotificationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Notifications.FindAsync([id], ct);

    public async Task<IReadOnlyList<NotificationEntity>> GetByUserIdAsync(
        string userId, int page, int pageSize, CancellationToken ct = default)
        => await dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<NotificationEntity>> GetAllAsync(
        int page, int pageSize, CancellationToken ct = default)
        => await dbContext.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default)
        => await dbContext.Notifications.CountAsync(n => n.UserId == userId, ct);

    public async Task<int> CountAllAsync(CancellationToken ct = default)
        => await dbContext.Notifications.CountAsync(ct);

    public async Task<int> DeleteOlderThanAsync(DateTimeOffset cutoff, CancellationToken ct = default)
    {
        var toDelete = await dbContext.Notifications
            .Where(n => n.CreatedAt < cutoff)
            .ToListAsync(ct);

        dbContext.Notifications.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync(ct);
        return toDelete.Count;
    }

    public async Task UpdateAsync(NotificationEntity notification, CancellationToken ct = default)
    {
        dbContext.Notifications.Update(notification);
        await dbContext.SaveChangesAsync(ct);
    }
}
