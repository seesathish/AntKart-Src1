using AK.Products.Domain.Common;
namespace AK.Products.Domain.Events;

public record ProductDeletedEvent(string ProductId) : IDomainEvent;
