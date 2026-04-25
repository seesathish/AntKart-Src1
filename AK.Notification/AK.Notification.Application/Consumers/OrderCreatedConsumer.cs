using AK.BuildingBlocks.Messaging.IntegrationEvents;
using AK.Notification.Application.Commands;
using AK.Notification.Application.Templates;
using AK.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace AK.Notification.Application.Consumers;

public sealed class OrderCreatedConsumer(IMediator mediator) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var msg = context.Message;
        var itemSummaries = msg.Items
            .Select(i => $"{i.Quantity}x {i.Sku} @ ₹{i.UnitPrice:N2}")
            .ToList();

        await mediator.Send(new SendNotificationCommand(
            msg.UserId,
            NotificationChannel.Email,
            NotificationTemplateType.OrderConfirmation,
            msg.CustomerEmail,
            new OrderConfirmationModel(msg.CustomerName, msg.OrderNumber, msg.TotalAmount, itemSummaries)),
            context.CancellationToken);
    }
}
