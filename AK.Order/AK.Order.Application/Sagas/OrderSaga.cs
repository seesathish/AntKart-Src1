using AK.BuildingBlocks.Messaging.IntegrationEvents;
using MassTransit;

namespace AK.Order.Application.Sagas;

public sealed class OrderSaga : MassTransitStateMachine<OrderSagaState>
{
    public State StockPending { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    public Event<OrderCreatedIntegrationEvent> OrderCreated { get; private set; } = null!;
    public Event<StockReservedIntegrationEvent> StockReserved { get; private set; } = null!;
    public Event<StockReservationFailedIntegrationEvent> StockReservationFailed { get; private set; } = null!;

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => StockReserved, e => e.CorrelateById(m => m.Message.OrderId));
        Event(() => StockReservationFailed, e => e.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(OrderCreated)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.CustomerEmail = ctx.Message.CustomerEmail;
                    ctx.Saga.CustomerName = ctx.Message.CustomerName;
                    ctx.Saga.OrderNumber = ctx.Message.OrderNumber;
                    ctx.Saga.TotalAmount = ctx.Message.TotalAmount;
                })
                .TransitionTo(StockPending));

        During(StockPending,
            When(StockReserved)
                .Publish(ctx => new OrderConfirmedIntegrationEvent(
                    ctx.Saga.OrderId,
                    ctx.Saga.UserId,
                    ctx.Saga.CustomerEmail,
                    ctx.Saga.CustomerName,
                    ctx.Saga.OrderNumber,
                    ctx.Saga.TotalAmount))
                .TransitionTo(Confirmed)
                .Finalize(),

            When(StockReservationFailed)
                .Publish(ctx => new OrderCancelledIntegrationEvent(
                    ctx.Saga.OrderId,
                    ctx.Saga.UserId,
                    ctx.Saga.CustomerEmail,
                    ctx.Saga.CustomerName,
                    ctx.Saga.OrderNumber,
                    ctx.Message.Reason))
                .TransitionTo(Cancelled)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
