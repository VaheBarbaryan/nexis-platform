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

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
