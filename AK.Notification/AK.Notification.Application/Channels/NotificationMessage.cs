using AK.Notification.Domain.Enums;

namespace AK.Notification.Application.Channels;

public sealed record NotificationMessage(
    string RecipientAddress,
    string? Subject,
    string Body,
    NotificationChannel Channel);
