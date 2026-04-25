namespace AK.Notification.Application.DTOs;

public sealed record NotificationDto(
    Guid Id,
    string UserId,
    string Channel,
    string TemplateType,
    string Status,
    string RecipientAddress,
    string? Subject,
    DateTimeOffset? SentAt,
    DateTimeOffset CreatedAt,
    string? ErrorMessage);
