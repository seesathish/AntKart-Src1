namespace AK.BuildingBlocks.Messaging.IntegrationEvents;

public sealed record UserRegisteredIntegrationEvent(
    string UserId,
    string CustomerEmail,
    string CustomerName) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
