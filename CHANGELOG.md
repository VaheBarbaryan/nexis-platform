# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog and follows Semantic Versioning.

## [Unreleased]

### Notes
- Project is under active development and not production-ready

### Added
- `VerifyUserEmailEndpoint` for user email verification
- `VerifyEmailAsync` method in `RegisterUserService` with token hash validation
- `UserEmailMustNotBeAlreadyVerifiedRule` to enforce email verification business rule
- Unit tests for `VerifyEmailAsync` covering all main scenarios
- `Modules.Users.Application.UnitTests` project and `tests` folder for application layer tests

### Changed
- Disabled CA1707 and CA1859 warnings in `.editorconfig` for test files

---

## [0.1.0-alpha] - 2026-01-30

### Added
- Initial Users module implementation
- User registration HTTP endpoint
- Transactional outbox pattern for domain event publishing (PostgreSQL)

### Changed
- Established modular monolith project structure

### Notes
- Initial developer preview release
- Not intended for production usage
