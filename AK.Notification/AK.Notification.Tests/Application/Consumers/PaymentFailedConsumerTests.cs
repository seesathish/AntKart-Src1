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

public class PaymentFailedConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly PaymentFailedConsumer _consumer;

    public PaymentFailedConsumerTests()
    {
        _consumer = new PaymentFailedConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var @event = new PaymentFailedIntegrationEvent(
            Guid.NewGuid(), Guid.NewGuid(), "user-2", "dave@example.com", "Dave",
            "ORD-20260301-ABCD", "Card declined");

        var contextMock = new Mock<ConsumeContext<PaymentFailedIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TemplateType.Should().Be(NotificationTemplateType.PaymentFailed);
        capturedCommand.RecipientAddress.Should().Be("dave@example.com");

        var model = capturedCommand.Model.Should().BeOfType<PaymentFailedModel>().Subject;
        model.Reason.Should().Be("Card declined");
        model.OrderNumber.Should().Be("ORD-20260301-ABCD");
    }
}
