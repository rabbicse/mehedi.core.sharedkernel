# Performance Standards

Backend:
- Async everywhere
- Use pagination
- Use projections
- Use caching
- Avoid blocking calls

Database:
- Optimize indexes
- Avoid SELECT *
- Avoid N+1 queries

Messaging:
- Prefer async workflows
- Use batching

Frontend:
- SSR first
- Avoid unnecessary hydration
- Lazy load heavy modules

Monitoring:
- Track slow queries
- Track API latency

Avoid:
- Chatty APIs
- Large payloads
- Synchronous cross-service calls