namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents a base event.
/// </summary>
#pragma warning disable CA1708 // Identifiers should differ by more than case
public abstract record BaseDomainEvent(string? messageType, string aggregateId) : IDomainEvent
#pragma warning restore CA1708 // Identifiers should differ by more than case
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public string? MessageType { get; protected init; } = messageType;

    /// <summary>
    /// Gets the aggregate ID as a string, supporting Guid, long, or any other key type.
    /// </summary>
    public string AggregateId { get; protected init; } = aggregateId;

    /// <summary>
    /// Gets the date and time when the event occurred (UTC).
    /// Defaults to <see cref="TimeProvider.System"/>; subclasses may override via
    /// <c>with { OccurredOn = timeProvider.GetUtcNow().UtcDateTime }</c>.
    /// </summary>
    public DateTime OccurredOn { get; protected init; } = TimeProvider.System.GetUtcNow().UtcDateTime;

}
