using AK.Order.Application.Common.Interfaces;
using AK.Order.Application.Common.Mapping;
using AK.Order.Application.Common.DTOs;
using MediatR;

namespace AK.Order.Application.Features.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await uow.Orders.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        order.UpdateStatus(request.NewStatus);
        await uow.Orders.UpdateAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        order.ClearDomainEvents();
        return OrderMapper.ToDto(order);
    }
}
