# ADR-003: Use Apache Kafka for Asynchronous Integration Events

**Date:** 2026-01-31

**Status:** Accepted

---

## Context

Nexis is designed as a modular monolith with a planned evolution toward distributed services. Several business workflows require **asynchronous cross-module communication**, including:

- Email verification after user registration
- Notifications fan-out
- Future feed processing and background jobs
- External system integrations

Direct synchronous communication between modules or services introduces:

- Tight coupling
- Increased latency
- Failure propagation
- Poor scalability under high load

Additionally, Nexis targets **2000+ RPS sustained throughput** and background-heavy workloads, requiring a messaging system that supports:

- High throughput
- Horizontal scalability
- Message durability
- Event replay
- Consumer group parallelism

---

## Decision

Apache Kafka is selected as the primary **integration event broker**.

The system will use the following event flow:

1. Domain events are converted into domain notifications and stored in the Outbox table within the same database transaction.
2. A background Outbox Processor reads pending notifications and publishes them via MediatR.
3. Notification handlers transform domain notifications into integration events and publish them to Kafka topics.
4. Consumer modules (Email, Notifications, Analytics) subscribe to Kafka topics using consumer groups.
5. Each consumer processes events asynchronously using background workers.
6. Consumers must be idempotent to handle retries and duplicate delivery.

> Note: Kafka will be used only for **integration events**, not for in-process domain event handling.

---

## Alternatives Considered

### RabbitMQ

Rejected because:

- Queue-oriented design rather than event streaming
- Limited event replay capability
- Lower throughput compared to Kafka
- More suitable for task queues than event-driven systems

---

### Direct HTTP Communication

Rejected because:

- Tight runtime coupling between services
- Difficult retry and failure handling
- Increased latency
- Poor resilience under partial outages

---

### AWS SQS

Rejected because:

- Vendor lock-in
- No native stream replay
- Limited consumer parallelism patterns
- Less suitable for event-driven architectures

---

### In-Memory Messaging (MediatR Only)

Rejected because:

- Cannot cross process boundaries
- No durability
- Not suitable for distributed architecture evolution

---

## Consequences

### Positive

- Decouples producer and consumer modules
- Enables horizontal scaling using consumer groups
- Supports event replay and recovery
- High throughput suitable for large-scale workloads
- Simplifies migration to microservices architecture
- Improves system resilience and fault isolation

---

### Negative

- Additional operational complexity
- Requires Kafka cluster management
- Requires schema governance and versioning discipline
- Consumers must handle idempotency and duplicate events
- More complex local development environment

---

## Implementation Notes

- Kafka producers publish only after successful Outbox persistence.
- Kafka consumers commit offsets only after successful processing.
- Integration events are versioned and backward compatible.
- JSON serialization is used initially, with future support for Avro and Schema Registry planned.
- Dead-letter handling will be implemented for permanently failed messages.

---

## Related Decisions

- ADR-001: Modular Monolith Architecture
- ADR-002: Transactional Outbox Pattern
