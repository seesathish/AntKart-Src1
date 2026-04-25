using AK.Notification.Domain.Enums;

namespace AK.Notification.Application.Channels;

public interface INotificationChannel
{
    NotificationChannel Channel { get; }
    Task SendAsync(NotificationMessage message, CancellationToken ct = default);
}
