using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using MediatR;

namespace AK.Notification.Application.Commands;

public sealed record SendNotificationCommand(
    string UserId,
    NotificationChannel Channel,
    NotificationTemplateType TemplateType,
    string RecipientAddress,
    NotificationTemplateModel Model) : IRequest<Guid>;
