using AK.Notification.Application.Queries;
using AK.Notification.Application.Repositories;
using AK.Notification.Domain.Enums;
using FluentAssertions;
using Moq;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Tests.Application.Queries;

public class GetUserNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _repoMock = new();
    private readonly GetUserNotificationsQueryHandler _handler;

    public GetUserNotificationsQueryHandlerTests()
    {
        _handler = new GetUserNotificationsQueryHandler(_repoMock.Object);
    }

    private static NotificationEntity CreateNotification(string userId = "user-1") =>
        NotificationEntity.Create(userId, NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail, "test@example.com", "Subject", "Body");

    [Fact]
    public async Task Handle_ReturnsPagedResultsForCorrectUserId()
    {
        var notifications = new List<NotificationEntity> { CreateNotification(), CreateNotification() };
        _repoMock.Setup(r => r.GetByUserIdAsync("user-1", 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        _repoMock.Setup(r => r.CountByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var query = new GetUserNotificationsQuery("user-1", 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyWhenNoNotifications()
    {
        _repoMock.Setup(r => r.GetByUserIdAsync("user-99", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationEntity>());
        _repoMock.Setup(r => r.CountByUserIdAsync("user-99", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetUserNotificationsQuery("user-99", 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
