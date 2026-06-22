# Getting Started

## Installation

```bash
dotnet add package Mehedi.Core.SharedKernel
```

Targets `net8.0` and `net10.0`.

## Quick 5-Minute Tour

### 1. Create an entity with a key

```csharp
public sealed class Product : BaseEntity<Guid>, IAggregateRoot
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    private Product() : base() { Name = string.Empty; }

    public static Product Create(string name, decimal price)
    {
        var product = new Product { Name = name, Price = price };
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, name));
        return product;
    }

    protected override Guid GenerateNewId() => Guid.NewGuid();
}
```

### 2. Define a domain event

```csharp
public record ProductCreatedEvent(Guid ProductId, string Name)
    : BaseDomainEvent(nameof(ProductCreatedEvent), ProductId);
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

Usage:
```csharp
var price1 = new Money(9.99m, "USD");
var price2 = new Money(9.99m, "USD");
Console.WriteLine(price1 == price2);  // False (== not overridden — use .Equals)
Console.WriteLine(price1.Equals(price2));  // True
```

### 4. Use a rich enumeration

```csharp
public sealed class OrderStatus : Enumerations
{
    public static readonly OrderStatus Pending   = new(1, "Pending");
    public static readonly OrderStatus Shipped   = new(2, "Shipped");
    public static readonly OrderStatus Delivered = new(3, "Delivered");

    private OrderStatus(int id, string name) : base(id, name) { }
}

// Lookup
var status = Enumerations.FromValue<OrderStatus>(2);       // Shipped
var all    = Enumerations.GetAll<OrderStatus>();            // all three
var sorted = all.Order().ToList();                          // by Id ascending
```

### 5. Implement a repository

```csharp
public interface IProductRepository : ICommandRepository<Product, Guid> { }
```

Implement it with EF Core or in-memory:

```csharp
public sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<Product> AddAsync(Product entity)
    {
        await db.Products.AddAsync(entity);
        return entity;
    }

    public async Task<Product> GetByIdAsync(Guid id) =>
        await db.Products.FindAsync(id) ?? throw new KeyNotFoundException(id.ToString());

    // ... remaining methods
    public void Dispose() { }
}
```

### 6. Wire up events with MediatR

```csharp
// Handler
public sealed class ProductCreatedHandler : INotificationHandler<ProductCreatedEvent>
{
    public Task Handle(ProductCreatedEvent n, CancellationToken ct)
    {
        Console.WriteLine($"Product created: {n.Name}");
        return Task.CompletedTask;
    }
}

// Dispatch after save
foreach (var evt in product.DomainEvents)
    await mediator.Publish(evt);
product.ClearDomainEvents();
```

## Common Patterns

| Pattern | Type to use |
|---|---|
| Entity with identity | `BaseEntity<TKey>` |
| Aggregate root | `BaseEntity<TKey>` + `IAggregateRoot` |
| Keyless entity (EF keyless) | `BaseEntity` |
| Immutable value type | `ValueObject` |
| Typed enum with behavior | `Enumerations` |
| Write repository | `ICommandRepository<TEntity, TKey>` |
| Read/query repository | `IQueryRepository<TQueryModel, TKey>` |
| Transaction boundary | `IUnitOfWork` |

## Running the Sample

```bash
cd samples/OrderManagement
dotnet run
```

See [samples/OrderManagement/](../samples/OrderManagement/) for the full Order aggregate example.
