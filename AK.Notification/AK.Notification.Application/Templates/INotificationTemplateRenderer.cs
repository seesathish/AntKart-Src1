using AK.Notification.Domain.Enums;

namespace AK.Notification.Application.Templates;

public interface INotificationTemplateRenderer
{
    NotificationContent Render(NotificationTemplateType type, NotificationTemplateModel model);
}

public sealed record NotificationContent(string Subject, string Body);
