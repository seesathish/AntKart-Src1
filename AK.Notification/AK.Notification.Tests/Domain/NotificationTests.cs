using AK.Notification.Domain.Enums;
using FluentAssertions;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Tests.Domain;

public class NotificationTests
{
    private static NotificationEntity CreateValid() =>
        NotificationEntity.Create(
            "user-1",
            NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail,
            "user@example.com",
            "Welcome!",
            "Hello, welcome to AntKart!");

    [Fact]
    public void Create_ValidInputs_ReturnsNotificationWithPendingStatus()
    {
        var notification = CreateValid();

        notification.Should().NotBeNull();
        notification.Status.Should().Be(NotificationStatus.Pending);
        notification.UserId.Should().Be("user-1");
        notification.RecipientAddress.Should().Be("user@example.com");
        notification.RetryCount.Should().Be(0);
    }

    [Fact]
    public void Create_EmptyUserId_ThrowsArgumentException()
    {
        var act = () => NotificationEntity.Create(
            "",
            NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail,
            "user@example.com",
            null,
            "body");

        act.Should().Throw<ArgumentException>().WithMessage("*UserId*");
    }

    [Fact]
    public void Create_EmptyRecipientAddress_ThrowsArgumentException()
    {
        var act = () => NotificationEntity.Create(
            "user-1",
            NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail,
            "",
            null,
            "body");

        act.Should().Throw<ArgumentException>().WithMessage("*RecipientAddress*");
    }

    [Fact]
    public void MarkSent_SetsStatusToSent_AndSentAt()
    {
        var notification = CreateValid();
        notification.MarkSent();

        notification.Status.Should().Be(NotificationStatus.Sent);
        notification.SentAt.Should().NotBeNull();
        notification.SentAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkFailed_SetsStatusToFailed_AndErrorMessage()
    {
        var notification = CreateValid();
        notification.MarkFailed("SMTP connection failed");

        notification.Status.Should().Be(NotificationStatus.Failed);
        notification.ErrorMessage.Should().Be("SMTP connection failed");
    }

    [Fact]
    public void IncrementRetry_IncrementsRetryCount()
    {
        var notification = CreateValid();
        notification.IncrementRetry();
        notification.IncrementRetry();

        notification.RetryCount.Should().Be(2);
    }

    [Fact]
    public void MarkSent_AlreadySent_ThrowsInvalidOperationException()
    {
        var notification = CreateValid();
        notification.MarkSent();

        var act = () => notification.MarkSent();
        act.Should().Throw<InvalidOperationException>().WithMessage("*already*sent*");
    }

    [Fact]
    public void Create_SetsCreatedAtToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;
        var notification = CreateValid();
        var after = DateTimeOffset.UtcNow;

        notification.CreatedAt.Should().BeOnOrAfter(before);
        notification.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var n1 = CreateValid();
        var n2 = CreateValid();

        n1.Id.Should().NotBe(n2.Id);
    }
}
