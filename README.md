[![NuGet](https://img.shields.io/nuget/v/Mehedi.Core.SharedKernel)](https://www.nuget.org/packages/Mehedi.Core.SharedKernel/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mehedi.Core.SharedKernel)](https://www.nuget.org/packages/Mehedi.Core.SharedKernel/)
[![CI](https://github.com/rabbicse/mehedi.core.sharedkernel/actions/workflows/publish.yml/badge.svg)](https://github.com/rabbicse/mehedi.core.sharedkernel/actions/workflows/publish.yml)

# Mehedi.Core.SharedKernel

Reusable base classes and interfaces for **Domain-Driven Design (DDD)** in .NET.
Targets **net8.0** and **net10.0** in a single NuGet package.

## Installation

```bash
dotnet add package Mehedi.Core.SharedKernel
```

## What's included

| Type | Purpose |
|------|---------|
| `BaseEntity<TKey>` | Abstract entity with typed key and domain event collection |
| `IAggregateRoot` | Marks an aggregate root; exposes `DomainEvents` / `ClearDomainEvents` |
| `BaseDomainEvent` | Abstract record for domain events; stores `MessageType`, `AggregateId` (string), `OccurredOn` |
| `IDomainEvent` | Thin wrapper over MediatR `INotification` |
| `ValueObject` | Abstract base with structural equality via `GetEqualityComponents()` |
| `Enumerations` | Rich enum base class with `GetAll<T>()`, `FromValue<T>()`, `FromDisplayName<T>()` |
| `ICommandRepository<T,TKey>` | Write-side repository contract with `CancellationToken` on all methods |
| `IQueryRepository<T,TKey>` | Read-side repository contract returning `PagedResult<T>` |
| `PagedResult<T>` | Wraps a page of results with `Total` count and `Items` list |
| `IUnitOfWork` | `SaveChangesAsync(CancellationToken)` |

## Quick start

### 1. Define an entity

```csharp
public class Order : BaseEntity<Guid>, IAggregateRoot
{
    public string CustomerName { get; private set; } = string.Empty;

    private Order() { }

    public static Order Place(string customerName)
    {
        var order = new Order { CustomerName = customerName };
        order.AddDomainEvent(new OrderPlacedEvent(order.Id, customerName));
        return order;
    }

    protected override Guid GenerateNewId() => Guid.NewGuid();
}
```

### 2. Define a domain event

```csharp
// AggregateId is string — works with Guid, long, or any key type
public record OrderPlacedEvent(Guid OrderId, string CustomerName)
    : BaseDomainEvent(nameof(OrderPlacedEvent), OrderId.ToString());
```

### 3. Define a value object

```csharp
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

### 4. Define a rich enumeration

```csharp
public sealed class OrderStatus : Enumerations
{
    public static readonly OrderStatus Pending   = new(1, "Pending");
    public static readonly OrderStatus Confirmed = new(2, "Confirmed");
    public static readonly OrderStatus Shipped   = new(3, "Shipped");

    private OrderStatus(int id, string name) : base(id, name) { }
}

// Usage
var all    = Enumerations.GetAll<OrderStatus>();
var status = Enumerations.FromValue<OrderStatus>(2);       // Confirmed
var same   = Enumerations.FromDisplayName<OrderStatus>("Shipped");
```

### 5. Implement a repository

```csharp
public class OrderRepository : ICommandRepository<Order, Guid>
{
    private readonly AppDbContext _db;
    public OrderRepository(AppDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Orders.FindAsync([id], ct);

    public async Task<Order> AddAsync(Order entity, CancellationToken ct = default)
    {
        await _db.Orders.AddAsync(entity, ct);
        return entity;
    }

    // ... other interface members
}
```

### 6. Dispatch domain events

```csharp
// After SaveChangesAsync, dispatch and clear events:
foreach (var evt in order.DomainEvents)
    await mediator.Publish(evt, cancellationToken);

order.ClearDomainEvents();
```

## .NET version matrix

| Feature | net8.0 | net10.0 |
|---------|--------|---------|
| All base classes | ✓ | ✓ |
| `TimeProvider` for `OccurredOn` | ✓ | ✓ |
| `AddDomainEvents(params ReadOnlySpan<IDomainEvent>)` | — | ✓ |

## Releasing a new version

```bash
git tag v1.2.0 && git push --tags        # stable release
git tag v1.2.0-rc.1 && git push --tags   # pre-release
```

CI builds both TFMs, runs all tests, then publishes a single `.nupkg` containing
`lib/net8.0/` and `lib/net10.0/` to NuGet.

## References

- [Ardalis.SharedKernel](https://github.com/ardalis/Ardalis.SharedKernel)
- [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture)
- [ASP.NET Core Clean Architecture CQRS](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
