# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Mehedi.Core.SharedKernel** is a reusable NuGet package providing base classes and interfaces for Domain-Driven Design (DDD) in .NET 8. It is published to nuget.org automatically on push to master.

## Commands

### Build
```powershell
dotnet build
```

### Run all tests
```powershell
dotnet test
```

### Run a single test file
```powershell
dotnet test --filter "FullyQualifiedName~BaseDomainEvent_Test"
```

### Run tests with coverage
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Pack NuGet package locally
```powershell
dotnet pack src/Mehedi.Core.SharedKernel/Mehedi.Core.SharedKernel.csproj --configuration Release
```

## Architecture

This library is a **shared kernel** for Clean Architecture / Vertical Slice Architecture applications. All types live in a single project under `src/Mehedi.Core.SharedKernel/`.

### Core Abstractions

**Entity layer:**
- `IEntity` / `IEntity<TKey>` — marker interfaces for entities
- `BaseEntity` / `BaseEntity<TKey>` — abstract base with domain event collection; subclasses must implement `GenerateNewId()`
- `IAggregateRoot` — marker interface; aggregate roots implement this alongside `BaseEntity<TKey>`

**Domain Events:**
- `IDomainEvent` — extends MediatR's `INotification` (thin wrapper to avoid direct MediatR coupling in domain)
- `BaseDomainEvent` — abstract `record` with `aggregateId`, `messageType`, and `OccurredOn` (UTC timestamp set on construction)

**Value Objects:**
- `ValueObject` — abstract base; subclasses implement `GetEqualityComponents()` to define equality by value rather than identity

**Rich Enumerations:**
- `Enumeration` — replaces C# enums with class-based enums supporting `GetAll<T>()`, `FromValue<T>()`, `FromDisplayName<T>()`

**Repository contracts:**
- `ICommandRepository<TEntity, TKey>` — write-side: Add, Update, Delete, GetById, GetAsync
- `IQueryRepository<TQueryModel, TKey>` — read-side with pagination support
- `IQueryModel` / `IQueryModel<TKey>` — marker interfaces for read DTOs
- `IUnitOfWork` — `SaveChangesAsync(CancellationToken)`

### Code Quality Rules

The build enforces strict quality gates — violations are treated as errors:
- Nullable reference types are enabled
- `AnalysisMode` is set to `All` (all Roslyn analyzers active)
- SonarAnalyzer.CSharp is included and runs on every build
- `EnforceCodeStyleInBuild` is enabled

Do not suppress warnings with `#pragma warning disable` in production code. In test code, existing suppressions for `CA1515` and `CA1707` are acceptable.

### Package Version

The version is defined in `src/Mehedi.Core.SharedKernel/Mehedi.Core.SharedKernel.csproj`. The CI pipeline auto-increments the patch version on each push to master. Do not manually bump the patch segment; bump major/minor only when warranted.

### Central Package Management

All NuGet package versions are declared in `Directory.Packages.props` at the root. Do not add `Version=` attributes to `<PackageReference>` elements inside `.csproj` files — they are managed centrally.
