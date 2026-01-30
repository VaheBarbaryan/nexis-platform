# ADR-001: Start With Modular Monolith Architecture

Date: 2026-01-30

Status: Accepted

## Context

Nexis targets approximately 2000 RPS sustained throughput and background-heavy workloads such as feed fanout and notification delivery.

Starting directly with microservices would introduce significant operational overhead, slower development feedback loops, and premature network boundaries without validated traffic patterns.

Initial project goals were:

- Fast local development and iteration
- Enforced domain isolation
- Clear migration path toward distributed architecture
- Reduced infrastructure and deployment complexity during early stages

## Decision

The system is implemented as a modular monolith following Domain-Driven Design principles.

Each module is isolated through:

- Separate application, domain, infrastructure, persistence, integration events, and API endpoint layers
- Independent project/assembly boundaries per module
- Explicit dependency direction rules (Domain → Application → Infrastructure)
- Inter-module communication restricted to domain events and integration events

The application entrypoint is limited to dependency wiring, middleware configuration, and infrastructure setup, and does not contain business logic.

## Alternatives Considered

### Microservices From Day One

Rejected due to:

- Increased deployment and debugging complexity
- Higher operational and infrastructure cost
- Slower iteration cycles
- Premature distribution without validated scaling requirements

### Traditional Layered Monolith

Rejected due to:

- Weak feature isolation
- High coupling between unrelated domains
- Increased difficulty when extracting services later

## Consequences

Positive:

- Faster development velocity and simpler local debugging
- Strong domain boundaries enforced at compile-time
- Easier automated testing and refactoring
- Straightforward extraction of modules into standalone services when required

Negative:

- All modules share the same runtime process and memory space
- Horizontal scaling applies to the whole application rather than individual domains
- Architectural discipline is required to prevent boundary erosion
