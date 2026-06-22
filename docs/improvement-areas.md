# Improvement Areas — Mehedi.Core.SharedKernel

Analyzed against the **architecture-designer**, **code-reviewer**, and **csharp-developer** skills.

---

## Priority Legend

| Symbol | Meaning |
|---|---|
| 🔴 Critical | Breaking/correctness issue — fix before next release |
| 🟠 High | Design flaw that will cause consumer pain |
| 🟡 Medium | Improvement with clear upside, no breaking change |
| 🟢 Low | Nice-to-have / minor polish |

---

## 1. API Contract Issues

### 🔴 `GetByIdAsync` returns non-nullable `TEntity`

```csharp
// ICommandRepository.cs — current
Task<TEntity> GetByIdAsync(TKey id);

// Correct
Task<TEntity?> GetByIdAsync(TKey id);
```

When the entity does not exist the implementation must either return `null` or throw. A non-nullable return type says "this never returns null," which is a lie. Consumers cannot safely handle missing entities without an exception path.

---

### 🔴 No `CancellationToken` on any repository method

Every async method in `ICommandRepository<TEntity, TKey>` and `IQueryRepository<TQueryModel, TKey>` omits `CancellationToken`. This makes the contracts unusable in ASP.NET Core endpoints, background services, or any cancellation-aware code path without wrapping.

```csharp
// Before
Task<TEntity> AddAsync(TEntity entity);

// After
Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
```

---

### 🟠 `ICommandRepository` extends `IDisposable`

Repositories should not own their lifetime. The `DbContext` (injected via DI) manages the connection; the `IUnitOfWork` wraps the transaction boundary. Disposing a repository in isolation violates the Unit of Work pattern and creates confusion for consumers.

**Recommendation:** Remove `IDisposable` from `ICommandRepository`. Let the consuming application's DI container manage the DbContext lifetime.

---

### 🟠 `DeleteByIdAsync` returns `TEntity`

```csharp
Task<TEntity> DeleteByIdAsync(TKey id);
```

Returning the deleted entity forces a pre-delete read in all implementations, even when the caller does not need the entity. Return `Task` (void) or `Task<bool>` instead.

---

### 🟠 `IQueryRepository` returns raw tuples

```csharp
Task<(long, IEnumerable<TQueryModel>)> GetAllCollectionAsync();
```

The `(long, IEnumerable<TQueryModel>)` tuple is weakly typed — consumers must rely on position, not name. Replace with a typed `PagedResult<T>`:

```csharp
public sealed record PagedResult<T>(long TotalCount, IEnumerable<T> Items);
```

---

### 🟠 `ICommandRepository.GetAsync` is on the write side

`GetAsync(Expression<Func<TEntity, bool>> predicate)` belongs conceptually on the query side. The command repository should focus on mutations. Move predicate-based querying to `IQueryRepository` or a dedicated `ISpecification<T>` interface.

---

### 🟡 `IUnitOfWork.SaveChangesAsync` returns `bool`

`bool` loses information (how many rows were affected). Standard EF Core pattern returns `int`. Throwing on failure (rather than returning `false`) is more idiomatic in C# — callers don't typically check `bool` return values from save operations.

```csharp
// After
Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
```

---

## 2. Domain Model Issues

### 🟠 `BaseEntity._domainEvents` holds `BaseDomainEvent` not `IDomainEvent`

```csharp
private readonly List<BaseDomainEvent> _domainEvents = [];
public IEnumerable<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
```

The abstraction `IDomainEvent` exists for decoupling, but the collection bypasses it. Any domain event that extends `IDomainEvent` directly (without going through `BaseDomainEvent`) cannot be added to an entity. Change both to `IDomainEvent`.

---

### 🟠 `BaseDomainEvent.aggregateId` is `Guid`-typed

```csharp
public abstract record BaseDomainEvent(string? messageType, Guid aggregateId)
```

This forces all aggregate roots to use `Guid` keys. A library targeting DDD should support string, long, or custom key types. Either:
- Change `aggregateId` to `string` (most flexible — any key type can `.ToString()`)
- Or make `BaseDomainEvent<TKey>` generic

---

### 🟡 `IAggregateRoot` has no domain-event contract

The `IAggregateRoot` marker interface says nothing about domain events. However, aggregate roots are the primary source of events in DDD. Consider adding:

```csharp
public interface IAggregateRoot
{
    IEnumerable<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
```

This would make aggregate roots self-describing and allow generic event-dispatch pipelines to work off the interface rather than the base class.

---

### 🟡 `BaseEntity` constructor calls the overridable `GenerateNewId()`

```csharp
// Suppressed with two pragma pairs
protected BaseEntity() => Id = GenerateNewId();
```

The constructor calling a virtual method is a well-known C# anti-pattern (CA2214 / S1699). If a subclass overrides `GenerateNewId()` and that override accesses a field not yet initialized, the behavior is undefined. A static factory method avoids this entirely:

```csharp
public static TEntity Create<TEntity>() where TEntity : BaseEntity<TKey>, new()
{
    var entity = new TEntity();
    entity.Id = entity.GenerateNewId();
    return entity;
}
```

---

### 🟡 `IDomainEvent` still directly extends MediatR's `INotification`

The stated goal is to remove direct MediatR dependency from the domain layer, but `IDomainEvent : INotification` means every domain assembly must reference MediatR transitively. One approach: remove the `INotification` base from `IDomainEvent` and provide an adapter/extension in a separate assembly (e.g., `Mehedi.Core.SharedKernel.MediatR`).

---

## 3. Value Object Issues

### 🟠 `ValueObject.GetHashCode()` uses XOR (`^`) aggregation

```csharp
return GetEqualityComponents()
    .Select(x => x != null ? x.GetHashCode() : 0)
    .Aggregate((x, y) => x ^ y);
```

XOR is commutative and self-cancelling: `{A, B}` and `{B, A}` produce the same hash, and `{A, A}` hashes to 0. This causes excessive collisions in hash-based collections (`Dictionary`, `HashSet`). Replace with `HashCode.Combine`:

```csharp
public override int GetHashCode()
{
    var hash = new HashCode();
    foreach (var component in GetEqualityComponents())
        hash.Add(component);
    return hash.ToHashCode();
}
```

---

### 🟡 `ValueObject.GetCopy()` returns nullable `ValueObject?`

```csharp
public ValueObject? GetCopy() => this.MemberwiseClone() as ValueObject;
```

`MemberwiseClone()` on `this` always succeeds — the `as` cast will never return null. The nullable return type misleads callers into null-checking unnecessarily. Return `ValueObject` (non-nullable) or the generic `T GetCopy<T>()`.

---

## 4. Enumerations Issues

### 🟠 `Enumerations.GetAll<T>()` uses uncached reflection

```csharp
public static IEnumerable<T> GetAll<T>() where T : Enumerations =>
    typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
             .Select(f => f.GetValue(null))
             .Cast<T>();
```

`GetAll<T>()` is called internally by `FromValue<T>()` and `FromDisplayName<T>()` — potentially on every request in a web application. Reflection on hot paths is expensive. Cache results in a static `ConcurrentDictionary<Type, object[]>`:

```csharp
private static readonly ConcurrentDictionary<Type, object[]> _cache = new();

public static IEnumerable<T> GetAll<T>() where T : Enumerations =>
    (T[])_cache.GetOrAdd(typeof(T), t =>
        t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
         .Select(f => f.GetValue(null)!)
         .ToArray());
```

---

### 🟡 Class is named `Enumerations` (plural) — convention mismatch

The Ardalis reference and general DDD convention use `Enumeration` (singular). The current plural name creates friction for consumers familiar with those libraries. Consider renaming in the next major version.

---

## 5. CI/CD Issues

### 🟠 Deprecated GitHub Actions versions

```yaml
uses: actions/checkout@v2.3.4           # current: v4
uses: brandedoutcast/publish-nuget@v2   # abandoned; use dotnet nuget push directly
```

`brandedoutcast/publish-nuget` is an unmaintained third-party action. Replace with the official `dotnet nuget push` command.

---

### 🟡 No test step in the publish workflow

The `publish.yml` workflow packs and publishes without running tests. A failing test set would still publish a broken package. Add a `dotnet test` step before the publish step.

---

### 🟡 Version bump happens before tests

The current flow bumps the version, commits, then publishes. If the publish step fails, you've committed a version bump for nothing. Reorder: test → bump → commit → publish.

---

## 6. Test Coverage Gaps

| Component | Covered Now | Missing |
|---|---|---|
| `BaseEntity` | Add/Clear domain events (partial) | `ClearDomainEvents`, multiple events, `GenerateNewId` |
| `BaseDomainEvent` | `OccurredOn` timestamp | `AggregateId`, `MessageType`, record equality |
| `ValueObject` | Value equality, hash code | Null handling, `GetCopy`, cross-type inequality |
| `Enumerations` | None | `GetAll`, `FromValue`, `FromDisplayName`, comparison operators, `AbsoluteDifference` |
| `ICommandRepository` | None | Contract tests |
| `IQueryRepository` | None | Contract/pagination tests |

---

## Summary Roadmap

| # | Area | Priority | Breaking? |
|---|---|---|---|
| 1 | Nullable return on `GetByIdAsync` | 🔴 | Yes (minor) |
| 2 | Add `CancellationToken` to repository interfaces | 🔴 | Yes |
| 3 | Fix `ValueObject.GetHashCode()` XOR | 🟠 | No |
| 4 | Cache `Enumerations.GetAll<T>()` | 🟠 | No |
| 5 | Remove `IDisposable` from `ICommandRepository` | 🟠 | Yes |
| 6 | Fix `DeleteByIdAsync` return type | 🟠 | Yes |
| 7 | Replace tuple with `PagedResult<T>` | 🟠 | Yes |
| 8 | Change `aggregateId` to `string` | 🟠 | Yes |
| 9 | Fix `_domainEvents` to use `IDomainEvent` | 🟠 | No |
| 10 | Update deprecated CI/CD actions | 🟠 | No |
| 11 | Add contract to `IAggregateRoot` | 🟡 | Yes |
| 12 | Fix `GetCopy()` nullability | 🟡 | No |
| 13 | Rename `Enumerations` → `Enumeration` | 🟡 | Yes (major) |
| 14 | Add test step to publish workflow | 🟡 | No |
| 15 | Remove MediatR from `IDomainEvent` | 🟡 | Yes (major) |
| 16 | Refactor virtual call in constructor | 🟡 | No |
