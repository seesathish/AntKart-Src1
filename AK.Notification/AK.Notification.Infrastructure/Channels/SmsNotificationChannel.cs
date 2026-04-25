using AK.Notification.Application.Channels;
using AK.Notification.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AK.Notification.Infrastructure.Channels;

internal sealed class SmsNotificationChannel(ILogger<SmsNotificationChannel> logger) : INotificationChannel
{
    public NotificationChannel Channel => NotificationChannel.Sms;

    public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        logger.LogInformation("SMS stub: would send to {Address}", message.RecipientAddress);
        return Task.CompletedTask;
    }
}
