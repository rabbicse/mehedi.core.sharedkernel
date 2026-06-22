# Architecture

## Type Hierarchy

```mermaid
classDiagram
    direction TB

    class IEntity {
        <<interface>>
    }

    class IEntityTKey {
        <<interface>>
        +TKey Id
    }

    class BaseEntity {
        <<abstract>>
        -List~BaseDomainEvent~ _domainEvents
        +IEnumerable~BaseDomainEvent~ DomainEvents
        #AddDomainEvent(BaseDomainEvent)
        +ClearDomainEvents()
    }

    class BaseEntityTKey {
        <<abstract>>
        +TKey Id
        #BaseEntity()
        #BaseEntity(TKey id)
        #GenerateNewId()* TKey
    }

    class IAggregateRoot {
        <<interface>>
    }

    IEntity <|-- IEntityTKey
    IEntityTKey <|.. BaseEntityTKey
    BaseEntity <|-- BaseEntityTKey
    IAggregateRoot <|.. BaseEntityTKey : implement together

    class IDomainEvent {
        <<interface>>
    }
    class INotification {
        <<interface>>
    }
    INotification <|-- IDomainEvent

    class BaseDomainEvent {
        <<abstract record>>
        +string? MessageType
        +Guid AggregateId
        +DateTime OccurredOn
    }
    IDomainEvent <|.. BaseDomainEvent

    class ValueObject {
        <<abstract>>
        #GetEqualityComponents()* IEnumerable~object~
        +Equals(object?) bool
        +GetHashCode() int
        +GetCopy() ValueObject?
        #EqualOperator(left, right)$ bool
        #NotEqualOperator(left, right)$ bool
    }

    class Enumerations {
        <<abstract>>
        +int Id
        +string Name
        +GetAll~T~()$ IEnumerable~T~
        +FromValue~T~(int)$ T
        +FromDisplayName~T~(string)$ T
        +AbsoluteDifference(a, b)$ int
        +CompareTo(object?) int
    }
```

---

## Domain Event Flow

```mermaid
sequenceDiagram
    participant Client
    participant Aggregate as BaseEntity&lt;TKey&gt;
    participant EventList as _domainEvents
    participant MediatR
    participant Handler

    Client->>Aggregate: Call business method (e.g., Place())
    Aggregate->>EventList: AddDomainEvent(new OrderPlacedEvent(...))
    Aggregate-->>Client: return aggregate

    Note over Client: After SaveChanges / UoW commit
    Client->>Aggregate: DomainEvents (read)
    Aggregate-->>Client: IEnumerable&lt;BaseDomainEvent&gt;
    loop each event
        Client->>MediatR: Publish(event)
        MediatR->>Handler: Handle(event, ct)
        Handler-->>MediatR: Task.CompletedTask
    end
    Client->>Aggregate: ClearDomainEvents()
```

---

## Repository + Unit of Work Pattern

```mermaid
graph TD
    AppService["Application Service"]
    CmdRepo["ICommandRepository&lt;TEntity, TKey&gt;"]
    QryRepo["IQueryRepository&lt;TQueryModel, TKey&gt;"]
    UoW["IUnitOfWork"]
    DB[("Database")]

    AppService -- "Write ops\n(Add/Update/Delete)" --> CmdRepo
    AppService -- "Read ops\n(Query/Paginate)" --> QryRepo
    AppService -- "SaveChangesAsync()" --> UoW
    CmdRepo -- "tracked entities" --> DB
    QryRepo -- "projections / read models" --> DB
    UoW -- "commit transaction" --> DB
```

---

## CQRS Separation

```mermaid
graph LR
    subgraph "Write Side (Command)"
        direction TB
        CmdRepo["ICommandRepository&lt;TEntity, TKey&gt;"]
        Entity["BaseEntity&lt;TKey&gt; + IAggregateRoot"]
        Events["Domain Events"]
        Entity -->|raises| Events
    end

    subgraph "Read Side (Query)"
        direction TB
        QryRepo["IQueryRepository&lt;TQueryModel, TKey&gt;"]
        QM["IQueryModel&lt;TKey&gt; — DTO / projection"]
        QryRepo --> QM
    end

    CmdRepo --- QryRepo
```

The two sides use **different models**: write side uses full aggregate entities; read side uses lightweight `IQueryModel` DTOs optimized for display.

---

## Multi-Target Framework Strategy

```mermaid
graph TD
    Lib["Mehedi.Core.SharedKernel\n.csproj\nTargetFrameworks: net8.0;net10.0"]
    Net8["net8.0 artifact\nMehedi.Core.SharedKernel.dll"]
    Net10["net10.0 artifact\nMehedi.Core.SharedKernel.dll"]
    NuPkg["NuGet Package\n.nupkg"]

    Lib --> Net8
    Lib --> Net10
    Net8 --> NuPkg
    Net10 --> NuPkg

    Consumer8["Consumer on .NET 8"]
    Consumer10["Consumer on .NET 10"]
    NuPkg --> Consumer8
    NuPkg --> Consumer10
```

Both TFM builds are packed into a single `.nupkg`. NuGet's `lib/` folder contains `lib/net8.0/` and `lib/net10.0/`. The runtime picks the best match automatically.

---

## Project Layout

```
mehedi.core.sharedkernel/
├── src/
│   └── Mehedi.Core.SharedKernel/     ← Published NuGet package
├── tests/
│   ├── UnitTests/                    ← Fast, isolated, no I/O
│   ├── IntegrationTests/             ← MediatR pipeline + repository contracts
│   ├── EndToEndTests/                ← Full DDD workflow scenarios
│   └── LoadTests/                    ← BenchmarkDotNet (run as Release exe)
├── samples/
│   └── OrderManagement/              ← Runnable console sample
└── docs/                             ← This documentation
```
