# Nexis Platform

Nexis is a high-throughput social platform backend demonstrating production-grade .NET system architecture.

## Tech Stack
- ASP.NET Core – backend framework
- PostgreSQL, MongoDB – relational & document storage
- Redis – caching & message brokering
- Kafka – event streaming
- Docker – containerized deployment

---

## Goals
- Handle 2,000 requests per second
- Horizontal scalability
- Fault-tolerant event processing

---

## Architecture

The system is structured as a **modular monolith** with clear module boundaries, making it straightforward to extract individual services when needed.

Key patterns in use:
- **Domain-Driven Design** — aggregates, domain events, value objects
- **Transactional Outbox** — reliable event publishing without dual-write issues
- **Outbox + Kafka** — at-least-once delivery with idempotent consumers

Detailed architecture diagrams and design decisions are available in the [`/docs`](./docs) directory.

---

## Claude Code Skills

Project-specific slash commands for code generation. Run from Claude Code CLI.

| Skill | Args | Generates |
|-------|------|-----------|
| `/gen-endpoint` | `POST posts CreatePost Posts --auth --body --response` | Endpoint class + Request/Response records + Validator |
| `/gen-domain-event` | `PostCreated Posts Post` | DomainEvent record + Notification + NotificationHandler (Kafka) |
| `/gen-entity` | `Story Posts aggregate` | Aggregate/Entity class + ValueObject Id + NotFoundException |
| `/gen-repository` | `Story Posts` | Repository interface + EF implementation + IEntityTypeConfiguration |
| `/gen-business-rule` | `ContentMaxLength Posts Post` | IBusinessRule implementation |
| `/gen-integration-event` | `StoryPublished Posts` | IntegrationEvent message record + optional consumer |
| `/gen-migration` | `AddStoryTable Posts` | Runs `dotnet ef migrations add` with correct flags for the module |
| `/gen-module` | `Notifications` | Full 6-layer module scaffold with all boilerplate |

---

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
