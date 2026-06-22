# .NET 8 DDD Microservice Standards

Architecture:
- DDD
- Clean Architecture
- CQRS
- Feature-first modularization

Preferred Structure:

src/
 ├── BuildingBlocks/
 ├── Services/
 │    ├── Identity/
 │    ├── AML/
 │    ├── KYC/
 │    └── Notification/

Each service contains:
- Domain
- Application
- Infrastructure
- API
- Features

Feature Structure:

Features/
 ├── MemberRegistration/
 │    ├── Commands/
 │    ├── Queries/
 │    ├── DTOs/
 │    ├── Validators/
 │    ├── Events/
 │    ├── Endpoints/
 │    └── Tests/

Rules:
- No business logic in controllers
- Domain layer must remain pure
- Infrastructure depends on domain
- Application coordinates use cases
- Use aggregate roots
- Use domain events
- Use async everywhere
- Use cancellation tokens
- Prefer immutable DTOs

Coding Style:
- Use records for DTOs
- Use sealed classes where appropriate
- Use explicit namespaces
- Prefer composition over inheritance
- Use Result pattern

API:
- RESTful
- Versioned APIs
- ProblemDetails for errors
- OpenAPI enabled

Dependency Injection:
- Use constructor injection
- Avoid service locator

Error Handling:
- Global exception middleware
- Structured logging
- Correlation IDs

Avoid:
- Generic repository pattern
- Massive shared kernel
- Fat services
- Anemic domain model

# Special guide
Try to use the following existing nuget packages. Reuse these following packages.

Mehedi.Application.SharedKernel
Mehedi.Core.SharedKernel
Mehedi.EventBus.Abstractions
Mehedi.EventBus.Kafka
Mehedi.Hangfire.Extensions
Mehedi.Patterns.Observer
Mehedi.Read.NoSql.Infrastructure
Mehedi.Write.RDBMS.Infrastructure



# Reference
https://github.com/rabbicse/aspdotnetcore-ddd-cleanarchitecture-microservices