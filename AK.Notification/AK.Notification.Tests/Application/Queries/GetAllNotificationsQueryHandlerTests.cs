using AK.Notification.Application.Queries;
using AK.Notification.Application.Repositories;
using AK.Notification.Domain.Enums;
using FluentAssertions;
using Moq;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Tests.Application.Queries;

public class GetAllNotificationsQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _repoMock = new();
    private readonly GetAllNotificationsQueryHandler _handler;

    public GetAllNotificationsQueryHandlerTests()
    {
        _handler = new GetAllNotificationsQueryHandler(_repoMock.Object);
    }

    private static NotificationEntity CreateNotification(string userId = "user-1") =>
        NotificationEntity.Create(userId, NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail, "test@example.com", "Subject", "Body");

    [Fact]
    public async Task Handle_ReturnsAllPagedResults()
    {
        var notifications = new List<NotificationEntity>
        {
            CreateNotification("user-1"),
            CreateNotification("user-2"),
            CreateNotification("user-3")
        };
        _repoMock.Setup(r => r.GetAllAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);
        _repoMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _handler.Handle(new GetAllNotificationsQuery(1, 20), CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyWhenNoNotifications()
    {
        _repoMock.Setup(r => r.GetAllAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NotificationEntity>());
        _repoMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new GetAllNotificationsQuery(1, 20), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
