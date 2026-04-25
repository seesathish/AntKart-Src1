using AK.BuildingBlocks.Messaging.IntegrationEvents;
using AK.Notification.Application.Commands;
using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace AK.Notification.Application.Consumers;

public sealed class PaymentSucceededConsumer(IMediator mediator) : IConsumer<PaymentSucceededIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededIntegrationEvent> context)
    {
        var msg = context.Message;
        await mediator.Send(new SendNotificationCommand(
            msg.UserId,
            NotificationChannel.Email,
            NotificationTemplateType.PaymentSucceeded,
            msg.CustomerEmail,
            new PaymentSucceededModel(msg.CustomerName, msg.OrderNumber, msg.Amount, msg.RazorpayPaymentId)),
            context.CancellationToken);
    }
}
