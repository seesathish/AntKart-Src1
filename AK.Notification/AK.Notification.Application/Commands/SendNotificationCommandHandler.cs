using AK.Notification.Application.Channels;
using AK.Notification.Application.Repositories;
using AK.Notification.Application.Templates;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Application.Commands;

public sealed class SendNotificationCommandHandler(
    INotificationRepository repository,
    INotificationChannelResolver channelResolver,
    INotificationTemplateRenderer templateRenderer,
    ILogger<SendNotificationCommandHandler> logger)
    : IRequestHandler<SendNotificationCommand, Guid>
{
    public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var content = templateRenderer.Render(request.TemplateType, request.Model);

        var notification = NotificationEntity.Create(
            request.UserId,
            request.Channel,
            request.TemplateType,
            request.RecipientAddress,
            content.Subject,
            content.Body);

        await repository.AddAsync(notification, cancellationToken);

        var channel = channelResolver.Resolve(request.Channel);
        var message = new NotificationMessage(
            notification.RecipientAddress,
            notification.Subject,
            notification.Body,
            notification.Channel);

        try
        {
            await channel.SendAsync(message, cancellationToken);
            notification.MarkSent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification {NotificationId} via {Channel}", notification.Id, request.Channel);
            notification.MarkFailed(ex.Message);
        }

        await repository.UpdateAsync(notification, cancellationToken);

        return notification.Id;
    }
}
