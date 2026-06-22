# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Mehedi.Core.SharedKernel** is a reusable NuGet package providing base classes and interfaces for Domain-Driven Design (DDD) in .NET. It publishes two packages to nuget.org on tag push:
- `Mehedi.Core.SharedKernel` — targets **net8.0**
- `Mehedi.Core.SharedKernel.Net10` — targets **net10.0** (includes net10-specific APIs)

The solution file is `Mehedi.Core.SharedKernel.slnx` (`.slnx` format, requires .NET 9+ SDK or VS 2022 17.10+).

## Commands

### Build
```powershell
dotnet build Mehedi.Core.SharedKernel.slnx
```

### Run all tests
```powershell
dotnet test Mehedi.Core.SharedKernel.slnx
```

### Run a single test file
```powershell
dotnet test --filter "FullyQualifiedName~BaseDomainEvent_Test"
```

### Run tests with coverage
```powershell
dotnet test Mehedi.Core.SharedKernel.slnx --collect:"XPlat Code Coverage"
```

### Pack NuGet packages locally
```powershell
dotnet pack src/Mehedi.Core.SharedKernel/Mehedi.Core.SharedKernel.csproj --configuration Release
dotnet pack src/Mehedi.Core.SharedKernel.Net10/Mehedi.Core.SharedKernel.Net10.csproj --configuration Release
```

## Architecture

Source lives in two projects under `src/`:
- `src/Mehedi.Core.SharedKernel/` — all `.cs` files, builds for net8.0
- `src/Mehedi.Core.SharedKernel.Net10/` — no `.cs` files; links source from the sibling directory via `<Compile Include="..\Mehedi.Core.SharedKernel\*.cs" />`, builds for net10.0

Test projects in `tests/` are multi-targeted (`net8.0;net10.0` via `Directory.Build.props`) and use conditional `<ProjectReference>` to reference the correct library project per TFM.

### Core Abstractions

**Entity layer:**
- `IEntity` / `IEntity<TKey>` — marker interfaces for entities
- `BaseEntity` / `BaseEntity<TKey>` — abstract base with domain event collection; subclasses must implement `GenerateNewId()`
- `IAggregateRoot` — exposes `DomainEvents` / `ClearDomainEvents()`; aggregate roots implement this alongside `BaseEntity<TKey>`

**Domain Events:**
- `IDomainEvent` — extends MediatR's `INotification` (thin wrapper to avoid direct MediatR coupling in domain)
- `BaseDomainEvent` — abstract `record` with `string aggregateId`, `messageType`, and `OccurredOn` (UTC via `TimeProvider`)

**Value Objects:**
- `ValueObject` — abstract base; subclasses implement `GetEqualityComponents()` to define equality by value rather than identity

**Rich Enumerations:**
- `Enumerations` — replaces C# enums with class-based enums supporting `GetAll<T>()`, `FromValue<T>()`, `FromDisplayName<T>()`; reflection results are cached

**Repository contracts:**
- `ICommandRepository<TEntity, TKey>` — write-side: Add, Update, Delete, GetByIdAsync (nullable), GetAsync; all methods take `CancellationToken`
- `IQueryRepository<TQueryModel, TKey>` — read-side returning `PagedResult<T>` with `CancellationToken`
- `PagedResult<T>` — wraps `Total` (long) and `Items` (IReadOnlyList); use `PagedResult.Empty<T>()` factory
- `IUnitOfWork` — `SaveChangesAsync(CancellationToken)`

**net10.0-only:**
- `BaseEntity.AddDomainEvents(params ReadOnlySpan<IDomainEvent>)` — batch add; compiled only under `#if NET9_0_OR_GREATER`

### Code Quality Rules

The build enforces strict quality gates — violations are treated as errors:
- Nullable reference types are enabled
- `AnalysisMode` is set to `All` (all Roslyn analyzers active)
- SonarAnalyzer.CSharp is included and runs on every build
- `EnforceCodeStyleInBuild` is enabled

Do not suppress warnings with `#pragma warning disable` in production code. In test code, existing suppressions for `CA1515` and `CA1707` are acceptable.

### Package Version

Version defaults to `0.0.0-local` in both source projects. CI injects the real version via `-p:Version=$TAG` at pack time. To release:

```bash
git tag v1.0.0 && git push --tags        # stable — Net8=1.0.0, Net10=2.0.0
git tag v1.0.0-rc.1 && git push --tags   # pre-release
```

Net10 version = (tag major + 1).minor.patch. Do not manually set version numbers in `.csproj` files.

### Central Package Management

All NuGet package versions are declared in `Directory.Packages.props` at the root. Do not add `Version=` attributes to `<PackageReference>` elements inside `.csproj` files — they are managed centrally.
