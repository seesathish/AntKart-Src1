using AK.Notification.Application.Channels;
using AK.Notification.Domain.Enums;

namespace AK.Notification.Infrastructure.Channels;

internal sealed class NotificationChannelResolver(IEnumerable<INotificationChannel> channels)
    : INotificationChannelResolver
{
    public INotificationChannel Resolve(NotificationChannel channel)
        => channels.Single(c => c.Channel == channel);
}
