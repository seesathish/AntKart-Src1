using AK.Products.Domain.Common;
namespace AK.Products.Domain.Events;

public record ProductUpdatedEvent(string ProductId, string ProductName) : IDomainEvent;
