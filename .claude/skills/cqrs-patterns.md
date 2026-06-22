# CQRS Standards

Commands:
- Mutate state
- Return Result or ID
- Validate before execution

Queries:
- Read-only
- Optimized projections
- Use AsNoTracking

Structure:

Application/
 ├── Abstractions/
 │    ├── ICommand.cs
 │    ├── ICommandHandler.cs
 │    ├── IQuery.cs
 │    └── IQueryHandler.cs

Example:

Features/
 ├── CreateMember/
 │    ├── CreateMemberCommand.cs
 │    ├── CreateMemberHandler.cs
 │    ├── CreateMemberValidator.cs

Rules:
- One handler per use case
- Avoid god handlers
- Keep handlers focused
- Prefer explicit orchestration

Validation:
- FluentValidation pipeline

Transactions:
- Commands transactional
- Queries non-transactional

Performance:
- Use projections
- Avoid loading full aggregates unnecessarily

Avoid:
- Shared mutable state
- Returning EF entities directly