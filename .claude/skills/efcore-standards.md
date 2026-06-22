# EF Core 8 Standards

Rules:
- Use AsNoTracking for reads
- Use projections
- Avoid lazy loading
- Use split queries where necessary
- Use compiled queries for hot paths

Performance:
- Avoid N+1 queries
- Use pagination
- Optimize indexes
- Use batching

Migrations:
- One migration per feature
- Review generated SQL

Entity Configuration:
- Use IEntityTypeConfiguration
- Separate configuration classes

Transactions:
- Explicit transaction boundaries

Preferred:
- Strongly typed IDs
- Value objects

Avoid:
- Generic repository abstraction
- Exposing DbContext outside infrastructure