# ADR-004: Continuous Integration Pipeline (GitHub Actions)

**Date**: 2026-02-05

**Status**: Accepted

---

## Context

The Nexis project now includes:

- HTTP endpoints (Register, Verify Email)
- Application-layer services
- Unit tests
- EditorConfig rules and analyzers

As the codebase grows, manual verification becomes unreliable. We need an automated way to ensure:

- The project builds successfully
- Unit tests pass
- Code quality rules are enforced
- Pull requests remain safe to merge

Without CI, regressions may be introduced unnoticed, especially as modules expand.

Even though the project is currently maintained by a single developer (me), introducing CI early prevents future technical debt and establishes a professional workflow.

---

## Decision

We will implement a GitHub Actions CI pipeline that runs on:

- push
- pull_request

The pipeline will:

1. Restore dependencies
2. Build the solution
3. Run analyzers via `dotnet build`
4. Execute all unit tests using `dotnet test`

Pipeline scope:

- Applies to the entire solution
- Must pass before merging pull requests
- Acts as a quality gate for future development

---

## Alternatives Considered

### No CI Pipeline (Manual Verification)

Rejected due to:

- Manual testing is error-prone
- Regressions may go unnoticed
- Not scalable as modules grow
- Does not reflect professional development workflow

---

### CI Introduced Later

Rejected due to:

- Harder to introduce after multiple modules exist
- Early discipline reduces future refactoring
- CI setup cost is minimal compared to long-term benefit

---

## Consequences

### Positive

- Prevents broken builds from being merged
- Ensures unit tests always run
- Enforces consistent code quality
- Establishes production-grade workflow early
- Improves confidence when refactoring

### Negative

- Slight increase in repository complexity
- Requires maintaining workflow configuration
- CI runs add small execution time to pull requests

---
