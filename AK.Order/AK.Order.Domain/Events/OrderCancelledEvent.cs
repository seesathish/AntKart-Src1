using AK.Order.Domain.Common;

namespace AK.Order.Domain.Events;

public sealed record OrderCancelledEvent(Guid OrderId, string UserId, string CustomerEmail, string CustomerName, string OrderNumber) : IDomainEvent;
