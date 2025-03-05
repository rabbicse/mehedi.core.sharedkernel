namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents a base event.
/// </summary>
#pragma warning disable CA1708 // Identifiers should differ by more than case
public abstract record BaseDomainEvent(string? messageType, Guid aggregateId) : IDomainEvent
#pragma warning restore CA1708 // Identifiers should differ by more than case
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public string? MessageType { get; protected init; } = messageType;

    /// <summary>
    /// Gets the aggregate ID.
    /// </summary>
    public Guid AggregateId { get; protected init; } = aggregateId;

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; private init; } = DateTime.UtcNow;

}
