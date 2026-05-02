Add an EF Core migration for a module. Arguments: `$ARGUMENTS`

Expected format: `<MigrationName> <Module>`

Example: `AddUserProfileTable Users`

## Command to run

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Modules/<Module>/Modules.<Module>.Persistence \
  --startup-project src/App \
  --context <Module>DbContext
```

Run from the solution root: `/home/vahe/RiderProjects/nexis-platform`

## Workflow

1. Run the migration command above
2. If it fails, diagnose: missing DbSet? entity not configured? connection issue?
3. After success, show the generated migration file so I can verify the SQL is correct
4. Flag any missing indexes, wrong column types, or missing `HasPrecision(0)` on DateTimeOffset columns

## Available modules and their DbContexts

| Module | DbContext |
|--------|-----------|
| Posts  | PostsDbContext |
| Users  | UsersDbContext |

## Rules
- Never run `dotnet ef database update` without explicit instruction — just generate the migration file
- EF Core uses snake_case naming (`UseSnakeCaseNamingConvention`) — generated SQL should reflect this
- Each module's migrations live under its own schema (Posts → `posts`, Users → `users`)
- DateTimeOffset columns should have `HasPrecision(0)` in configuration to avoid timestamp drift
