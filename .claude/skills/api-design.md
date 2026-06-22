# API Design Standards

REST Principles:
- Resource-oriented naming
- Correct HTTP verbs
- Stateless APIs

Responses:
- ProblemDetails for errors
- Consistent envelope format when needed

Pagination:
- Mandatory for large datasets

Filtering:
- Explicit query parameters

Versioning:
- URL or header versioning

Documentation:
- Swagger/OpenAPI required

Avoid:
- RPC-style endpoints
- Over-fetching
- Deeply nested responses