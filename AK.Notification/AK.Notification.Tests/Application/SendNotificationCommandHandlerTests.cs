using AK.Notification.Application.Channels;
using AK.Notification.Application.Commands;
using AK.Notification.Application.Repositories;
using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NotificationEntity = AK.Notification.Domain.Entities.Notification;

namespace AK.Notification.Tests.Application;

public class SendNotificationCommandHandlerTests
{
    private readonly Mock<INotificationRepository> _repoMock = new();
    private readonly Mock<INotificationChannelResolver> _resolverMock = new();
    private readonly Mock<INotificationChannel> _channelMock = new();
    private readonly Mock<INotificationTemplateRenderer> _rendererMock = new();
    private readonly SendNotificationCommandHandler _handler;

    public SendNotificationCommandHandlerTests()
    {
        _channelMock.Setup(c => c.Channel).Returns(NotificationChannel.Email);
        _channelMock.Setup(c => c.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _resolverMock.Setup(r => r.Resolve(NotificationChannel.Email)).Returns(_channelMock.Object);

        _rendererMock.Setup(r => r.Render(It.IsAny<NotificationTemplateType>(), It.IsAny<NotificationTemplateModel>()))
            .Returns(new NotificationContent("Test Subject", "Test Body"));

        _repoMock.Setup(r => r.AddAsync(It.IsAny<NotificationEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<NotificationEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new SendNotificationCommandHandler(
            _repoMock.Object,
            _resolverMock.Object,
            _rendererMock.Object,
            NullLogger<SendNotificationCommandHandler>.Instance);
    }

    private static SendNotificationCommand CreateCommand() => new(
        "user-1",
        NotificationChannel.Email,
        NotificationTemplateType.WelcomeEmail,
        "user@example.com",
        new WelcomeEmailModel("Test User"));

    [Fact]
    public async Task Handle_SuccessfulSend_MarksNotificationSent()
    {
        NotificationEntity? capturedNotification = null;
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<NotificationEntity>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationEntity, CancellationToken>((n, _) => capturedNotification = n)
            .Returns(Task.CompletedTask);

        await _handler.Handle(CreateCommand(), CancellationToken.None);

        capturedNotification.Should().NotBeNull();
        capturedNotification!.Status.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public async Task Handle_ChannelThrows_MarksNotificationFailed_DoesNotRethrow()
    {
        _channelMock.Setup(c => c.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP error"));

        NotificationEntity? capturedNotification = null;
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<NotificationEntity>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationEntity, CancellationToken>((n, _) => capturedNotification = n)
            .Returns(Task.CompletedTask);

        var act = async () => await _handler.Handle(CreateCommand(), CancellationToken.None);

        await act.Should().NotThrowAsync();
        capturedNotification.Should().NotBeNull();
        capturedNotification!.Status.Should().Be(NotificationStatus.Failed);
        capturedNotification.ErrorMessage.Should().Contain("SMTP error");
    }

    [Fact]
    public async Task Handle_SavesNotificationBeforeSending()
    {
        var callOrder = new List<string>();

        _repoMock.Setup(r => r.AddAsync(It.IsAny<NotificationEntity>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationEntity, CancellationToken>((_, _) => callOrder.Add("add"))
            .Returns(Task.CompletedTask);

        _channelMock.Setup(c => c.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationMessage, CancellationToken>((_, _) => callOrder.Add("send"))
            .Returns(Task.CompletedTask);

        await _handler.Handle(CreateCommand(), CancellationToken.None);

        callOrder.Should().ContainInOrder("add", "send");
    }

    [Fact]
    public async Task Handle_UsesCorrectChannel()
    {
        await _handler.Handle(CreateCommand(), CancellationToken.None);

        _resolverMock.Verify(r => r.Resolve(NotificationChannel.Email), Times.Once);
        _channelMock.Verify(c => c.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RendersTemplate()
    {
        var command = CreateCommand();
        await _handler.Handle(command, CancellationToken.None);

        _rendererMock.Verify(r => r.Render(command.TemplateType, command.Model), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsNotificationId()
    {
        var id = await _handler.Handle(CreateCommand(), CancellationToken.None);

        id.Should().NotBe(Guid.Empty);
    }
}
