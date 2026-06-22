# Event Driven Architecture Standards

Event Types:
- Domain Events
- Integration Events

Rules:
- Domain events stay inside service boundary
- Integration events cross service boundaries

Messaging:
- Kafka preferred
- RabbitMQ supported

Event Naming:
<Member><Action>Event

Examples:
- MemberRegisteredEvent
- LoanApprovedEvent

Event Structure:
- EventId
- Timestamp
- CorrelationId
- Payload

Patterns:
- Outbox pattern
- Retry policies
- Idempotent consumers

Avoid:
- Distributed transactions
- Tight coupling between services

Background Processing:
- Hosted services
- Channel queues
- Hangfire if necessary

Observability:
- Trace message flow
- Include correlation IDs