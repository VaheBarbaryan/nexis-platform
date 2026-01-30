# Nexis Architecture

Nexis is a high-throughput social platform backend designed as a modular monolith following Domain-Driven Design (DDD) principles.
It is built to support future scaling into microservices, reliable asynchronous processing, and maintainable domain boundaries.

---

## 1. Current Modules

> At this stage, only the Users module is partially implemented. Future modules are planned.

### Users
- Handles user registration, authentication, and profile management
- Organized in DDD layers: Domain, Application, Infrastructure, IntegrationEvents, Endpoints
- Publishes domain events via MediatR
- Uses PostgreSQL for user data
- Uses Redis for storing email verification tokens

### Planned Modules (for roadmap)
- **Auth**: JWT authentication and authorization
- **Posts**: Posts, feeds, and timelines
- **Notifications**: Async notifications, emails, in-app
- **SharedKernel**: Base entities, ValueObjects, DomainEvents, Result types

---

## 2. Module Structure

Each module follows this layering:

- **Domain** – Aggregates, Entities, ValueObjects, domain events
- **Application** – Use cases, commands/queries, services
- **Infrastructure** – EF Core persistence, repositories, external integrations
- **IntegrationEvents** – Outgoing events, event handlers
- **Endpoints** – API controllers or gRPC endpoints

The **SharedKernel** module provides base types and utilities shared across modules.

The **App** project serves only as the entrypoint:

- Configures dependency injection
- Registers middleware
- Starts the application
- Does not contain business logic

---

## 3. Communication

- **Intra-module**: Direct method calls within Application and Domain layers
- **Inter-module**: Domain events via MediatR within the monolith
- **Future async**: Transactional Outbox → Kafka for external service events

---

## 4. Infrastructure Overview

| Component  | Purpose                                           |
|------------|--------------------------------------------------|
| PostgreSQL | Core relational data (Users module, etc.)       |
|  MongoDB   | Planned for flexible, document-based data       |
| Redis      | Email verification token storage, caching       |
| Kafka      | Planned for async event distribution            |
| Docker     | Containerization for reproducible environments  |
| CI/CD      | Planned for automated deployment and testing    |

---

## 5. Design Principles

- **Module isolation**: No direct database access across modules
- **Bounded contexts**: Each module owns its data and logic
- **Single entrypoint**: App project only wires dependencies
- **Async processing**: Outbox pattern ensures reliable integration events
- **Future scalability**: Designed for horizontal scaling and microservice extraction

---

## 6. Diagrams

- Architecture diagram will be placed in `/docs/diagrams/architecture.png`
- Diagram will show modules, SharedKernel, App entrypoint, and event flow
