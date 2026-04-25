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

public class OrderConfirmedConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly OrderConfirmedConsumer _consumer;

    public OrderConfirmedConsumerTests()
    {
        _consumer = new OrderConfirmedConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var @event = new OrderConfirmedIntegrationEvent(
            Guid.NewGuid(), "user-4", "frank@example.com", "Frank",
            "ORD-20260501-IJKL", 450m);

        var contextMock = new Mock<ConsumeContext<OrderConfirmedIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TemplateType.Should().Be(NotificationTemplateType.OrderConfirmed);
        capturedCommand.RecipientAddress.Should().Be("frank@example.com");

        var model = capturedCommand.Model.Should().BeOfType<OrderConfirmedModel>().Subject;
        model.TotalAmount.Should().Be(450m);
        model.OrderNumber.Should().Be("ORD-20260501-IJKL");
    }
}
