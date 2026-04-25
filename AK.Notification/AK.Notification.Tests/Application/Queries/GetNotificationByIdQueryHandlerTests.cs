using AK.Notification.Application.Queries;
using AK.Notification.Application.Repositories;
using AK.Notification.Domain.Enums;
using FluentAssertions;
using Moq;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Tests.Application.Queries;

public class GetNotificationByIdQueryHandlerTests
{
    private readonly Mock<INotificationRepository> _repoMock = new();
    private readonly GetNotificationByIdQueryHandler _handler;

    public GetNotificationByIdQueryHandlerTests()
    {
        _handler = new GetNotificationByIdQueryHandler(_repoMock.Object);
    }

    private static NotificationEntity CreateNotification(string userId = "user-1") =>
        NotificationEntity.Create(userId, NotificationChannel.Email,
            NotificationTemplateType.WelcomeEmail, "test@example.com", "Subject", "Body");

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationEntity?)null);

        var result = await _handler.Handle(
            new GetNotificationByIdQuery(Guid.NewGuid(), "user-1"),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ReturnsDto_WhenFoundAndSameUser()
    {
        var notification = CreateNotification("user-1");
        _repoMock.Setup(r => r.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _handler.Handle(
            new GetNotificationByIdQuery(notification.Id, "user-1"),
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.UserId.Should().Be("user-1");
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorizedAccessException_WhenDifferentUser()
    {
        var notification = CreateNotification("user-1");
        _repoMock.Setup(r => r.GetByIdAsync(notification.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var act = async () => await _handler.Handle(
            new GetNotificationByIdQuery(notification.Id, "user-2"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
