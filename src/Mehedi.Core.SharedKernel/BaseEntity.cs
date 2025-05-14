namespace Mehedi.Core.SharedKernel;

/// <summary>
/// Represents an abstract base entity class.
/// </summary>
public abstract class BaseEntity<TKey> : BaseEntity, IEntity<TKey> where TKey : IEquatable<TKey>
{    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// </summary>
#pragma warning disable CA2214 // Do not call overridable methods in constructors
#pragma warning disable S1699 // Constructors should only call non-overridable methods
    protected BaseEntity() => Id = GenerateNewId();
#pragma warning restore S1699 // Constructors should only call non-overridable methods
#pragma warning restore CA2214 // Do not call overridable methods in constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected BaseEntity(TKey id) => Id = id;

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    public TKey Id { get; private init; }

    /// <summary>
    /// Generates a new identifier for the entity (override for specific types).
    /// </summary>
    protected abstract TKey GenerateNewId();
}

/// <summary>
/// BaseEntity without TKey
/// </summary>
public abstract class BaseEntity 
{
    private readonly List<BaseDomainEvent> _domainEvents = [];
    /// <summary>
    /// Gets the domain events associated with this entity.
    /// </summary>
    /// 
    public IEnumerable<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all the domain events associated with this entity.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
} 
