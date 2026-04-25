using AK.Notification.Domain.Enums;

namespace AK.Notification.Domain.Entities;

public sealed class Notification : Entity, IAggregateRoot
{
    public string UserId { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; }
    public NotificationTemplateType TemplateType { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string RecipientAddress { get; private set; } = string.Empty;
    public string? Subject { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public int RetryCount { get; private set; }

    private Notification() { }

    public static Notification Create(
        string userId,
        NotificationChannel channel,
        NotificationTemplateType templateType,
        string recipientAddress,
        string? subject,
        string body)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(recipientAddress))
            throw new ArgumentException("RecipientAddress cannot be empty.", nameof(recipientAddress));

        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Channel = channel,
            TemplateType = templateType,
            Status = NotificationStatus.Pending,
            RecipientAddress = recipientAddress,
            Subject = subject,
            Body = body,
            CreatedAt = DateTimeOffset.UtcNow,
            RetryCount = 0
        };
    }

    public void MarkSent()
    {
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("Notification is already marked as sent.");

        Status = NotificationStatus.Sent;
        SentAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed(string error)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = error;
    }

    public void IncrementRetry()
    {
        RetryCount++;
    }
}
