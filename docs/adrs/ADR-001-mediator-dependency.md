# ADR-001: MediatR as a Transitive Dependency

## Status
Accepted (open for revision in a future major version)

## Context

`IDomainEvent` extends MediatR's `INotification`. The stated intent was to shield domain assemblies from a direct MediatR dependency, but the current implementation still requires MediatR transitively because `IDomainEvent` lives in the same assembly as the `INotification` reference.

## Decision

Accept MediatR as a dependency of `Mehedi.Core.SharedKernel` for now. The benefit — domain events flow directly through MediatR's in-process bus without adapter code — outweighs the coupling cost for the typical consumer.

## Alternatives Considered

- **Remove `INotification` from `IDomainEvent`** and provide a separate adapter assembly (`Mehedi.Core.SharedKernel.MediatR`). Consumers would need two packages. Adds complexity for little gain unless the library targets teams using non-MediatR buses.
- **Use a custom `IEventBus` abstraction** in the library. Over-engineering for the current scope.

## Consequences

- **Positive:** Zero adapter code for consumers using MediatR (the vast majority).
- **Negative:** Consumers on non-MediatR pipelines (e.g., Wolverine, NServiceBus) need a thin adapter.
- **Future option:** In v2.0, split into `Mehedi.Core.SharedKernel` (no MediatR) + `Mehedi.Core.SharedKernel.MediatR` (bridge package).

## Trade-offs

Simplicity for the common case over flexibility for edge cases.
