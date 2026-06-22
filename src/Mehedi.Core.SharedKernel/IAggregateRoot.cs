namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Marker interface for aggregate roots.
/// Aggregate roots own a collection of domain events that are dispatched after the
/// unit of work completes.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>Domain events raised by this aggregate since the last dispatch.</summary>
    IEnumerable<IDomainEvent> DomainEvents { get; }

    /// <summary>Clears all pending domain events (call after dispatching).</summary>
    void ClearDomainEvents();
}
