# ADR 001: Database per Service

Status: Proposed

Context

- Each microservice should own its data to reduce coupling and allow independent scaling and deployment.

Decision

- Use one PostgreSQL database per service. Each database will be initialized via Docker init scripts placed under docker/init-db.

Consequences

- Services must replicate data via events when other services' data is needed (read models).
