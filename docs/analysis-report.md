# Codebase Analysis Report — Mehedi.Core.SharedKernel

Generated: 2026-06-22

---

## Project Identity

| Field | Value |
|---|---|
| Package ID | Mehedi.Core.SharedKernel |
| Current Version | 1.0.6 |
| Target Framework | .NET 8.0 |
| License | Apache 2.0 |
| Published To | nuget.org |
| CI Trigger | Push to master (src/** changes) or manual dispatch |

---

## Repository Structure

```
mehedi.core.sharedkernel/
├── .github/workflows/publish.yml     # CI/CD: auto version bump + nuget publish
├── src/
│   └── Mehedi.Core.SharedKernel/
│       ├── BaseDomainEvent.cs
│       ├── BaseEntity.cs
│       ├── Enumeration.cs
│       ├── IAggregateRoot.cs
│       ├── ICommandRepository.cs
│       ├── IDomainEvent.cs
│       ├── IEntity.cs
│       ├── IQueryModel.cs
│       ├── IQueryRepository.cs
│       ├── IUnitOfWork.cs
│       ├── ValueObject.cs
│       ├── Mehedi.Core.SharedKernel.csproj
│       └── nuget.config
├── tests/
│   └── Mehedi.Core.SharedKernel.UnitTests/
│       ├── BaseDomainEventTests/
│       ├── BaseEntityTests/
│       └── ValueObjectTests/
├── Directory.Build.props              # Global build settings
├── Directory.Packages.props           # Central NuGet version management
├── Mehedi.Core.SharedKernel.sln
└── CLAUDE.md
```

---

## Architecture Overview

This is a **shared kernel library** designed to underpin DDD-based .NET applications following Clean Architecture or Vertical Slice Architecture patterns. All production code lives in a single assembly.

### Entity Hierarchy

```
IEntity
  └── IEntity<TKey>
        └── BaseEntity<TKey>   (abstract, manages domain event list)
              └── BaseEntity   (no-key variant for EF keyless scenarios)

IAggregateRoot                 (marker interface — implement alongside BaseEntity<TKey>)
```

`BaseEntity<TKey>` holds a private `List<IDomainEvent>` exposed via `DomainEvents`. Subclasses must implement the abstract `GenerateNewId()` method.

### Domain Event Hierarchy

```
MediatR.INotification
  └── IDomainEvent
        └── BaseDomainEvent (abstract record)
              - aggregateId  : string
              - messageType  : string
              - OccurredOn   : DateTime (UTC, set on construction)
```

Using a `record` for `BaseDomainEvent` gives structural equality for free, which matters for deduplication scenarios.

### Value Object

`ValueObject` is an abstract class. Subclasses implement `GetEqualityComponents()` returning an `IEnumerable<object>`. Equality and hash code are derived entirely from those components. Includes `GetCopy()` (MemberwiseClone).

### Rich Enumeration

`Enumeration` replaces standard C# enums with a class-based pattern:
- Static instances discovered via reflection (`GetAll<T>()`)
- `FromValue<T>(int value)` and `FromDisplayName<T>(string name)` factory methods
- Implements `IComparable`

### Repository Contracts

**Write side — `ICommandRepository<TEntity, TKey>`**
- `AddAsync(entity)` / `AddRangeAsync(entities)`
- `UpdateAsync(entity)` / `UpdateRangeAsync(entities)`
- `DeleteAsync(entity)` / `DeleteRangeAsync(entities)` / `DeleteByIdAsync(id)`
- `GetByIdAsync(id)` / `GetAsync(expression)`

**Read side — `IQueryRepository<TQueryModel, TKey>`**
- `GetAllCollectionAsync()`
- `GetCollectionAsync(pageNumber, pageSize)`
- `GetByIdAsync(id)`
- Query models implement `IQueryModel` / `IQueryModel<TKey>`

**Unit of Work — `IUnitOfWork`**
- `SaveChangesAsync(CancellationToken)`

---

## Key Dependencies

| Package | Version | Purpose |
|---|---|---|
| MediatR | 12.4.1 | `IDomainEvent` extends `INotification` |
| SonarAnalyzer.CSharp | 10.7.0 | Static code analysis |
| xUnit | 2.5.3 | Unit test framework |
| FluentAssertions | 6.12.0 | Test assertions |
| coverlet.collector | 6.0.0 | Code coverage |
| Microsoft.NET.Test.SDK | 17.8.0 | Test runner |

---

## Build Quality Gates

Configured in `Directory.Build.props`:

| Setting | Value |
|---|---|
| Nullable | enable |
| ImplicitUsings | enable |
| AnalysisLevel | latest-All |
| TreatWarningsAsErrors | true |
| CodeAnalysisTreatWarningsAsErrors | true |
| EnforceCodeStyleInBuild | true |

SonarAnalyzer.CSharp is included as an `<Analyzer>` reference — it runs on every build, not just in CI.

---

## CI/CD Pipeline

File: `.github/workflows/publish.yml`

1. Triggered on push to `master` (path filter: `src/**`) or `workflow_dispatch`
2. Setup .NET 8.0.x
3. Auto-increment patch version in `.csproj`
4. Commit version bump as `github-actions[bot]`
5. `dotnet pack` → `dotnet nuget push` to nuget.org using `NUGET_API_KEY` secret

---

## Inspirations / References

- [ardalis/Ardalis.SharedKernel](https://github.com/ardalis/Ardalis.SharedKernel)
- [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
- [jeangatto ASP.NET Core CQRS Event Sourcing](https://github.com/jeangatto)
