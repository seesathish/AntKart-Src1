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

public class OrderCancelledConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly OrderCancelledConsumer _consumer;

    public OrderCancelledConsumerTests()
    {
        _consumer = new OrderCancelledConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var @event = new OrderCancelledIntegrationEvent(
            Guid.NewGuid(), "user-3", "eve@example.com", "Eve",
            "ORD-20260401-EFGH", "Item out of stock");

        var contextMock = new Mock<ConsumeContext<OrderCancelledIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TemplateType.Should().Be(NotificationTemplateType.OrderCancelled);
        capturedCommand.RecipientAddress.Should().Be("eve@example.com");

        var model = capturedCommand.Model.Should().BeOfType<OrderCancelledModel>().Subject;
        model.Reason.Should().Be("Item out of stock");
        model.OrderNumber.Should().Be("ORD-20260401-EFGH");
    }
}
