using AK.Products.Domain.Common;
namespace AK.Products.Domain.Events;

public sealed record ProductCreatedEvent(string ProductId, string ProductName) : IDomainEvent;
