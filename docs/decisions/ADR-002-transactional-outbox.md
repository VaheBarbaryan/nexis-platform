# ADR-002: Implement Transactional Outbox Pattern

Date: 2026-01-30

Status: Accepted

## Context

Nexis requires reliable asynchronous processing for events such as email verification, notifications, and future integrations (Posts feed, background jobs, etc.).
Domain events must be published only if the corresponding database transaction succeeds, ensuring **data consistency** between aggregates and integration events.

Challenges:

- Avoid losing domain events when a transaction fails
- Avoid duplicate events being published
- Ensure high throughput for potential 2000+ RPS
- Minimize coupling between modules and external services

---

## Decision

Implement the **Transactional Outbox Pattern**:

1. Create an `outbox_messages` table in PostgreSQL to store serialized domain events within the same transaction as aggregate changes.
2. A background dispatcher reads pending events from the Outbox table in batches.
3. Dispatcher publishes events asynchronously via MediatR (internal) and plans for Kafka (external).
4. Implement retry mechanism with logging for failed dispatches.
5. Integration ensures eventual consistency across modules and future microservices.
> Note: We store domain notifications, which wrap domain events with additional metadata, in the outbox to support reliable internal and external consumption.

---

## Alternatives Considered

### Direct Event Publishing

- Publish events immediately after saving aggregates.

Rejected due to:

- risk of lost events if transaction rolls back.

### Eventual Consistency Without Outbox

- Let external systems retry failed operations.

Rejected due to:

- Increases operational complexity, difficult to reason about system state.

### External Queue Only (Kafka or RabbitMQ)

- Write directly to message broker.

Rejected due to:

- Violates atomicity with database transaction; risk of lost events on failure.

---

## Consequences

**Positive:**

- Guarantees domain events are stored reliably and only published after successful transactions.
- Simplifies eventual migration to microservices with reliable async communication.
- Decouples modules from external event consumers.
- Batch processing enables higher throughput with minimal performance impact.

**Negative:**

- Requires additional table and background processing infrastructure.
- Slight operational overhead for polling Outbox table.
- Must handle retry and idempotency correctly to avoid duplicate events.

---
