# ADR-005: Module-Based Database Schemas

**Date**: 2026-02-28

**Status**: Accepted

---

## Context

The Nexis application initially used a single `public` schema in PostgreSQL containing all tables.

At the time of this decision:

- Only the `Users` module exists
- All tables were previously located in the `public` schema
- Migration history was limited and the system is not in production

As Nexis evolves into a modular system with multiple bounded contexts (e.g., Users, Posts, Notifications, Billing), keeping all tables in a single schema introduces architectural risks:

- Weak separation between modules at the persistence layer
- Increased risk of accidental cross-module coupling
- Harder ownership boundaries
- Reduced clarity in migrations and database structure
- Misalignment between application modules and database structure

Because the system is not yet in production and migration history is limited, it is safe to reset migrations and establish a new database baseline that reflects proper modular boundaries.

---

## Decision

We will adopt module-based PostgreSQL schemas.

Each application module will own its own database schema.

Example mapping:

| Module        | PostgreSQL Schema |
|---------------|-------------------|
| Users         | `users`           |
| Posts         | `posts`           |
| Notifications | `notifications`   |
| Billing       | `billing`         |

Implementation details:

- The `public` schema will no longer store domain tables.
- The `Users` module tables are now created under the `users` schema.
- A new initial migration establishes this as the baseline.
- ORM configuration must explicitly define schema ownership.
- Cross-schema foreign keys are allowed but must be explicit and intentional.

The database structure must reflect the modular architecture of the application.

---

## Alternatives Considered

### Continue Using Single `public` Schema

Rejected due to:

- Implicit coupling between modules
- Schema pollution as the system grows
- Harder reasoning about ownership
- Poor alignment with clean architecture principles

---

### Separate Physical Databases Per Module

Rejected at this stage due to:

- Increased operational complexity
- More difficult transaction management
- Infrastructure overhead
- Premature optimization for current scale

This may be reconsidered if Nexis evolves into distributed microservices.

---

## Consequences

### Positive

- Stronger modular boundaries at persistence level
- Clear table ownership per module
- Easier reasoning about migrations
- Better scalability
- Improved architectural discipline
- Prepares system for possible future service decomposition

### Negative

- Slightly more complex ORM configuration
- Cross-schema references require explicit handling
- Reset migration history requires team awareness
- Additional discipline required when adding new modules

---

## Notes

- The migration history was intentionally reset to establish a clean baseline.
- All future modules must define their own schema explicitly.
- The `public` schema must not be used for domain tables going forward.
