using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Application.DTOs;

internal static class NotificationMapper
{
    internal static NotificationDto ToDto(this NotificationEntity n) =>
        new(
            n.Id,
            n.UserId,
            n.Channel.ToString(),
            n.TemplateType.ToString(),
            n.Status.ToString(),
            n.RecipientAddress,
            n.Subject,
            n.SentAt,
            n.CreatedAt,
            n.ErrorMessage);
}
