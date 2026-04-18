using AK.Order.Application.Common.Interfaces;
using MediatR;

namespace AK.Order.Application.Features.CancelOrder;

public sealed class CancelOrderCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await uow.Orders.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        order.Cancel();
        await uow.Orders.UpdateAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        order.ClearDomainEvents();
        return true;
    }
}
