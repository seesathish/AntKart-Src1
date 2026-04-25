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

public class OrderCreatedConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly OrderCreatedConsumer _consumer;

    public OrderCreatedConsumerTests()
    {
        _consumer = new OrderCreatedConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var items = new List<OrderItemPayload>
        {
            new("prod-1", "MEN-SHIR-001", 2, 500m),
            new("prod-2", "WOM-DRES-001", 1, 800m)
        };
        var @event = new OrderCreatedIntegrationEvent(
            Guid.NewGuid(), "user-1", "bob@example.com", "Bob",
            "ORD-20260101-ABCD", items, 1800m);

        var contextMock = new Mock<ConsumeContext<OrderCreatedIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TemplateType.Should().Be(NotificationTemplateType.OrderConfirmation);
        capturedCommand.RecipientAddress.Should().Be("bob@example.com");

        var model = capturedCommand.Model.Should().BeOfType<OrderConfirmationModel>().Subject;
        model.OrderNumber.Should().Be("ORD-20260101-ABCD");
        model.TotalAmount.Should().Be(1800m);
        model.ItemSummaries.Should().HaveCount(2);
        model.ItemSummaries[0].Should().Contain("MEN-SHIR-001");
    }
}
