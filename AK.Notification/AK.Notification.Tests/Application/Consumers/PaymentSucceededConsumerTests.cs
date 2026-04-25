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

public class PaymentSucceededConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly PaymentSucceededConsumer _consumer;

    public PaymentSucceededConsumerTests()
    {
        _consumer = new PaymentSucceededConsumer(_mediatorMock.Object);
    }

    [Fact]
    public async Task ConsumeAsync_PublishesCorrectCommand()
    {
        var @event = new PaymentSucceededIntegrationEvent(
            Guid.NewGuid(), Guid.NewGuid(), "user-1", "carol@example.com", "Carol",
            "ORD-20260201-WXYZ", 999.99m, "pay_rzp123");

        var contextMock = new Mock<ConsumeContext<PaymentSucceededIntegrationEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        contextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        SendNotificationCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Guid>>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Guid>, CancellationToken>((cmd, _) => capturedCommand = cmd as SendNotificationCommand)
            .ReturnsAsync(Guid.NewGuid());

        await _consumer.Consume(contextMock.Object);

        capturedCommand.Should().NotBeNull();
        capturedCommand!.TemplateType.Should().Be(NotificationTemplateType.PaymentSucceeded);
        capturedCommand.RecipientAddress.Should().Be("carol@example.com");

        var model = capturedCommand.Model.Should().BeOfType<PaymentSucceededModel>().Subject;
        model.Amount.Should().Be(999.99m);
        model.RazorpayPaymentId.Should().Be("pay_rzp123");
        model.OrderNumber.Should().Be("ORD-20260201-WXYZ");
    }
}
