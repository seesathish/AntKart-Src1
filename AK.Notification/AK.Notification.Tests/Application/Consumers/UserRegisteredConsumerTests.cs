using AK.BuildingBlocks.Messaging.IntegrationEvents;
using AK.Notification.Application.Commands;
using AK.Notification.Application.Consumers;
using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using FluentAssertions;
using MassTransit;
using MediatR;
using Moq;

namespace AK.Notification.Tests.Application.Consumers;

public class UserRegisteredConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly UserRegisteredConsumer _consumer;

    public UserRegisteredConsumerTests()
    {
        _consumer = new UserRegisteredConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var @event = new UserRegisteredIntegrationEvent("user-1", "alice@example.com", "Alice");
        var contextMock = new Mock<ConsumeContext<UserRegisteredIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.UserId.Should().Be("user-1");
        capturedCommand.Channel.Should().Be(NotificationChannel.Email);
        capturedCommand.TemplateType.Should().Be(NotificationTemplateType.WelcomeEmail);
        capturedCommand.RecipientAddress.Should().Be("alice@example.com");
        capturedCommand.Model.Should().BeOfType<WelcomeEmailModel>();
        ((WelcomeEmailModel)capturedCommand.Model).CustomerName.Should().Be("Alice");
    }
}
