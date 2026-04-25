using AK.Notification.Domain.Enums;

namespace AK.Notification.Application.Channels;

public interface INotificationChannelResolver
{
    INotificationChannel Resolve(NotificationChannel channel);
}
