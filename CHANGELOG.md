## [unreleased]

### 🚀 Features

- *(users)* Implement Users module with User, Role, Permission aggregates and migrations
- *(users)* Add modular seeding infrastructure
- *(users)* Add registration logic
- *(users)* Integrate Kafka producer for notifications topic
- *(emails-infrastructure)* Add Emails module with Kafka consumer and email sending
- *(users-application)* Send SendEmailIntegrationCommand on UserCreatedNotification
- *(users)* Store email verification token keyed by token hash
- *(domain)* Introduce base exceptions for semantic categorization
- *(app)* Implement GlobalExceptionHandler using ProblemDetails
- *(users)* Add user email verification logic
- *(users)* Add login endpoint with cookie-based JWT
- *(users)* Add authenticated /api/me endpoint
- *(users)* Add logout endpoint with cookie clearing and hashed refresh token removal
- *(users)* Add refresh token rotation endpoint
- *(users)* Implement resend email verification functionality
- *(users)* Implement forgot password functionality
- Add ApplyMigrations extension method for automatic database migrations
- Add Docker configuration for development environment
- *(users)* Implement password reset flow with token validation
- *(users)* Implement change password flow
- *(posts)* Add posts module with authors and posts table
- *(shared)* Add `KafkaConsumerConfigFactory`, topics and consume logging
- *(users)* Raise `UserEmailVerifiedDomainEvent` and publish `UserEventMessage` to kafka
- *(posts)* Sync from user.events kafka topics
- *(posts)* Add post create endpoint
- *(posts)* Add update post endpoint with author ownership validation
- *(posts)* Add delete post endpoint with author ownership validation
- *(posts)* Add get post by id endpoint
- *(posts)* Implement cursor-based paginated get posts endpoint
- *(posts)* Implement like/unlike endpoints with likes count on get
- *(infra)* Extract shared infra and wire outbox + events for Posts module
- *(posts)* Add Comments aggregate with full CRUD and Kafka events
- *(posts)* Add comments count and author info to posts
- *(posts)* Add follow/unfollow feature
- *(posts)* Add followers/following endpoints and global /api prefix

### 🐛 Bug Fixes

- *(users)* Provide correct return types and params for IEmailVerificationTokenStore
- *(users)* Prevent BackgroundService from blocking host startup
- *(users)* Store userId instead of notification id in redis
- *(posts)* Resolve central package management version conflict in unit test project
- *(posts)* Remove xunit version from unit test csproj for central package management
- *(posts)* Add missing FluentValidation DI extensions package reference

### 🚜 Refactor

- *(users-infrastructure)* Introduce IKafkaProducerOptions and IKafkaConsumerOptions
- *(users)* Generalize token store logic with keyed services
- *(users)* [**breaking**] Enfore module schema and aggregate relationships

### 📚 Documentation

- *(adr)* Add ADR-001 for modular monolith architecture decision
- Add architecture overview for Nexis platform
- *(adr)* Add ADR-002 for transactional outbox with domain notifications
- *(adr)* Add ADR-003 for Kafka-based integration events
- *(adr)* Add ADR-004 for Github CI pipeline
- Add license badge to README
- *(adr)* Add module-based database schema decision record
- Add transactional outbox pattern diagram

### 🧪 Testing

- *(register-user)* Add unit tests for VerifyEmailAsync
- *(register-user)* Add unit tests for RegisterAsync
- *(users)* Add unit tests for LoginAsync
- *(users)* Add unit tests for refresh token rotation
- *(users)* Add unit tests for resend email verification
- *(users)* Add unit tests for password service
- *(users)* Add unit tests for reset password flow
- *(users)* Add unit tests for change password flow
- *(users)* Update unit tests to mock correct unit of work instance
- *(posts)* Add post aggregate unit tests
- *(posts)* Add unit tests for post update method
- *(posts)* Add unit tests for post delete method
- *(posts)* Add unit tests for post like/unlike logic
- *(posts)* Add unit tests for Comment domain and CommentService
- *(posts)* Add unit tests for Follow aggregate and FollowService

### ⚙️ Miscellaneous Tasks

- Add editorconfig, directory build props, and sonar analyzer
- Set CA1812 severity to suggestion
- Reorganize solution folders and projects
- Add GitHub Actions pipeline with build and test workflow
- Add Makefile for Docker development workflow
- Add MIT license
- Add external kafka listener for local development
- Add git-cliff changelog automation
- *(changelog)* Update changelog
- Add automated PR labeler
- Change .github files extensions to .yml
- *(changelog)* Update changelog
- Add commitlint workflow with husky local hook
- *(changelog)* Update changelog
- Add nuget vulnerability scan workflow
- *(changelog)* Update changelog
- Add codeql security analysis workflow
- *(changelog)* Update changelog
- Add kafka-init topic creation and update solution
- *(shared)* Add Confluent.Kafka nuget package
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- Update tests solution folder
- *(changelog)* Update changelog
- Add CLAUDE.md with project guidance for Claude Code
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
- *(changelog)* Update changelog
