using AK.Order.Domain.Common;

namespace AK.Order.Domain.Events;

public sealed record OrderCreatedEvent(Guid OrderId, string UserId, string OrderNumber) : IDomainEvent;
