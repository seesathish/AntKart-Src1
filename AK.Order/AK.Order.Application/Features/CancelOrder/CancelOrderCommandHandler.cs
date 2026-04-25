using AK.BuildingBlocks.Messaging.IntegrationEvents;
using AK.Order.Application.Common.Interfaces;
using MassTransit;
using MediatR;

namespace AK.Order.Application.Features.CancelOrder;

public sealed class CancelOrderCommandHandler(IUnitOfWork uow, IPublishEndpoint publisher)
    : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await uow.Orders.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        order.Cancel();
        await uow.Orders.UpdateAsync(order, ct);

        await publisher.Publish(new OrderCancelledIntegrationEvent(
            order.Id,
            order.UserId,
            order.CustomerEmail,
            order.CustomerName,
            order.OrderNumber,
            "Cancelled by customer"), ct);

        await uow.SaveChangesAsync(ct);
        order.ClearDomainEvents();
        return true;
    }
}
