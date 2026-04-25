namespace AK.Notification.Infrastructure.Services;

public sealed record NotificationSettings
{
    public int RetentionDays { get; init; } = 90;
}
