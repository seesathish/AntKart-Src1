using AK.Notification.Application.Channels;
using AK.Notification.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AK.Notification.Infrastructure.Channels;

internal sealed class WhatsAppNotificationChannel(ILogger<WhatsAppNotificationChannel> logger) : INotificationChannel
{
    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        logger.LogInformation("WhatsApp stub: would send to {Address}", message.RecipientAddress);
        return Task.CompletedTask;
    }
}
