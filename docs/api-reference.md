# API Reference

## BaseEntity

### `BaseEntity` (keyless)

Abstract base for entities without a surrogate key (e.g., EF Core keyless entities).

| Member | Description |
|---|---|
| `DomainEvents` | `IEnumerable<BaseDomainEvent>` — read-only view of pending events |
| `AddDomainEvent(evt)` | Protected. Appends an event to the pending queue |
| `ClearDomainEvents()` | Public. Removes all pending events — call after dispatching |

### `BaseEntity<TKey>` where `TKey : IEquatable<TKey>`

Extends `BaseEntity` with a typed identity key.

| Member | Description |
|---|---|
| `Id` | `TKey` — set once via constructor or `GenerateNewId()` |
| `GenerateNewId()` | Abstract. Override to control ID generation |
| `BaseEntity()` | Calls `GenerateNewId()` — auto-assign ID on creation |
| `BaseEntity(TKey id)` | Explicit ID — for reconstitution from persistence |

**Example implementations:**

```csharp
// Guid key (most common)
public sealed class Order : BaseEntity<Guid>, IAggregateRoot
{
    protected override Guid GenerateNewId() => Guid.NewGuid();
}

// Sequential long key
public sealed class SequenceEntity : BaseEntity<long>
{
    protected override long GenerateNewId() => 0; // assigned by DB on insert
}
```

---

## IAggregateRoot

Empty marker interface. Implement alongside `BaseEntity<TKey>` to signal that a type is an aggregate root.

```csharp
public sealed class Order : BaseEntity<Guid>, IAggregateRoot { ... }
```

---

## BaseDomainEvent

`public abstract record BaseDomainEvent(string? messageType, Guid aggregateId) : IDomainEvent`

| Property | Type | Description |
|---|---|---|
| `MessageType` | `string?` | Discriminator — typically `nameof(EventClass)` |
| `AggregateId` | `Guid` | ID of the aggregate that raised this event |
| `OccurredOn` | `DateTime` | UTC timestamp — set automatically on construction |

**Defining an event:**

```csharp
public record OrderPlacedEvent(Guid OrderId, string Customer)
    : BaseDomainEvent(nameof(OrderPlacedEvent), OrderId);
```

Because `BaseDomainEvent` is a `record`, equality is structural — two `OrderPlacedEvent` instances with the same `OrderId` and `Customer` are equal.

---

## IDomainEvent

`public interface IDomainEvent : MediatR.INotification`

Thin wrapper enabling domain events to flow through MediatR's `IPublisher.Publish(INotification)` pipeline without the domain assembly needing to reference MediatR directly (it is a transitive dependency).

---

## ValueObject

`public abstract class ValueObject`

| Member | Description |
|---|---|
| `GetEqualityComponents()` | Abstract. Yield each field that participates in equality |
| `Equals(object?)` | Compares component sequences; type-safe |
| `GetHashCode()` | Derived from components via XOR aggregation |
| `GetCopy()` | Shallow clone via `MemberwiseClone()` |
| `EqualOperator(l, r)` | Protected static — helper for `==` operator in subclasses |
| `NotEqualOperator(l, r)` | Protected static — helper for `!=` operator in subclasses |

**Implementing a value object:**

```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    // Optional: expose == / != operators by delegating to helpers
    public static bool operator ==(Money left, Money right) => EqualOperator(left, right);
    public static bool operator !=(Money left, Money right) => NotEqualOperator(left, right);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

> **Note:** `GetHashCode()` currently uses XOR — see [improvement areas](improvement-areas.md#4-enumerations-issues) for the collision risk and the `HashCode.Combine` fix.

---

## Enumerations

`public abstract class Enumerations : IComparable`

| Member | Description |
|---|---|
| `Id` | `int` — numeric discriminator |
| `Name` | `string` — display name |
| `GetAll<T>()` | Static. Returns all static instances via reflection |
| `FromValue<T>(int)` | Static. Lookup by Id; throws `InvalidOperationException` if not found |
| `FromDisplayName<T>(string)` | Static. Lookup by Name (case-sensitive); throws if not found |
| `AbsoluteDifference(a, b)` | Static. `Math.Abs(a.Id - b.Id)` |
| `CompareTo(object?)` | Compares by `Id`; throws on null or wrong type |
| `==`, `!=`, `<`, `<=`, `>`, `>=` | All operator overloads based on `Id` |

**Defining an enumeration:**

```csharp
public sealed class Priority : Enumerations
{
    public static readonly Priority Low    = new(1, "Low");
    public static readonly Priority Medium = new(2, "Medium");
    public static readonly Priority High   = new(3, "High");

    private Priority(int id, string name) : base(id, name) { }
}

// Usage
var p = Enumerations.FromValue<Priority>(2);  // Medium
var all = Enumerations.GetAll<Priority>();     // Low, Medium, High
Console.WriteLine(Priority.High > Priority.Low);  // true
```

> **Caution:** `GetAll<T>()` uses reflection and is **not cached**. On hot paths (e.g., per-request lookups), cache the result yourself until a caching fix is applied.

---

## ICommandRepository\<TEntity, TKey\>

Write-side repository contract.

```csharp
public interface ICommandRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity>               AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>>  AddAsync(IEnumerable<TEntity> entity);
    Task<TEntity>               UpdateAsync(TEntity entity);
    Task<IEnumerable<TEntity>>  UpdateAsync(IEnumerable<TEntity> entity);
    Task                        DeleteAsync(TEntity entity);
    Task                        DeleteAsync(IEnumerable<TEntity> entity);
    Task<TEntity>               DeleteByIdAsync(TKey id);
    Task<TEntity>               GetByIdAsync(TKey id);
    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
}
```

> **Known issues:** No `CancellationToken`, `GetByIdAsync` is non-nullable, `IDisposable` on the repository. See [improvement areas](improvement-areas.md).

---

## IQueryRepository\<TQueryModel, TKey\>

Read-side repository contract. Works with lightweight `IQueryModel` DTOs.

```csharp
public interface IQueryRepository<TQueryModel, in TKey>
    where TQueryModel : IQueryModel<TKey>
    where TKey : IEquatable<TKey>
{
    Task<(long, IEnumerable<TQueryModel>)> GetAllCollectionAsync();
    Task<(long, IEnumerable<TQueryModel>)> GetCollectionAsync(int pageNumber = 1, int pageSize = 100);
    Task<TQueryModel?>                     GetByIdAsync(TKey id);
}
```

The tuple `(long total, IEnumerable<T> items)` will be replaced with a `PagedResult<T>` record in a future version.

---

## IQueryModel

Marker interfaces for read-side DTOs.

```csharp
public interface IQueryModel { }
public interface IQueryModel<out TKey> : IQueryModel where TKey : IEquatable<TKey>
{
    TKey Id { get; }
}
```

**Example:**

```csharp
public sealed record OrderSummary(Guid Id, string CustomerName, decimal Total)
    : IQueryModel<Guid>;
```

---

## IUnitOfWork

```csharp
public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

Returns `true` when at least one entity was persisted. Typically implemented by the EF Core `DbContext`.
