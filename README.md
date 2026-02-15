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
The system is implemented as a modular monolith following DDD principles, designed to migrate to microservices as demand grows.

Detailed architecture and design decisions are available in the `/docs` directory.

---

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
