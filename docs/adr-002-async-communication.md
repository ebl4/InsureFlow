# ADR 002: Asynchronous Communication via RabbitMQ

Status: Proposed

Context

- Synchronous HTTP calls between services create runtime coupling and availability risks.

Decision

- Use RabbitMQ for publishing domain events. PropostaService will publish PropostaStatusAlteradoEvent when status changes. ContratacaoService will consume and maintain a local read-model cache.

Consequences

- Reduced synchronous dependencies; eventual consistency is introduced and must be handled by consumers.
