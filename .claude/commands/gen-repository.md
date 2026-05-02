Generate repository interface, implementation, and EF configuration for an entity. Arguments: `$ARGUMENTS`

Expected format: `<EntityName> <Module>`

Example: `Story Posts`

## Files to generate

### 1. Repository interface — `src/Modules/<Module>/Modules.<Module>.Domain/<EntityName>/Repositories/I<EntityName>Repository.cs`

```csharp
using Modules.<Module>.Domain.<EntityName>.ValueObjects;

namespace Modules.<Module>.Domain.<EntityName>.Repositories;

public interface I<EntityName>Repository
{
    Task<List<<EntityName>>> GetAsync(string? cursor, int limit = 20, CancellationToken ct = default);

    Task<<EntityName>?> GetByIdAsync(<EntityName>Id id, CancellationToken ct = default);

    void Add(<EntityName> entity);
}
```

Adjust methods based on actual query needs — ask if unclear.

### 2. Repository implementation — `src/Modules/<Module>/Modules.<Module>.Persistence/Repositories/<EntityName>Repository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Modules.<Module>.Domain.<EntityName>;
using Modules.<Module>.Domain.<EntityName>.Repositories;
using Modules.<Module>.Domain.<EntityName>.ValueObjects;
using Modules.<Module>.Persistence.Contexts;
using SharedKernel.Application.Pagination;

namespace Modules.<Module>.Persistence.Repositories;

public sealed class <EntityName>Repository : I<EntityName>Repository
{
    private readonly <Module>DbContext _context;

    public <EntityName>Repository(<Module>DbContext context)
    {
        _context = context;
    }

    public async Task<List<<EntityName>>> GetAsync(string? cursor, int limit = 20, CancellationToken ct = default)
    {
        var decodedCursor = Cursor.Decode(cursor);

        if (decodedCursor is null)
        {
            return await _context.<EntityName>s
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Take(limit + 1)
                .ToListAsync(ct);
        }

        var lastDate = decodedCursor.Date;
        var lastId = decodedCursor.LastId;

        // NOTE: soft delete filter does not apply to FromSqlInterpolated — applied manually
        return await _context.<EntityName>s
            .FromSqlInterpolated($"""
                                  SELECT * FROM <schema>.<table>
                                  WHERE deleted_at IS NULL
                                    AND (created_at, id) < ({lastDate}, {lastId})
                                  ORDER BY created_at DESC, id DESC
                                  LIMIT {limit + 1}
                                  """)
            .ToListAsync(ct);
    }

    public async Task<<EntityName>?> GetByIdAsync(<EntityName>Id id, CancellationToken ct = default)
    {
        return await _context.<EntityName>s.SingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public void Add(<EntityName> entity)
    {
        _context.<EntityName>s.Add(entity);
    }
}
```

### 3. EF configuration — `src/Modules/<Module>/Modules.<Module>.Persistence/Configurations/<EntityName>Configuration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.<Module>.Domain.<EntityName>;
using Modules.<Module>.Domain.<EntityName>.ValueObjects;

namespace Modules.<Module>.Persistence.Configurations;

public sealed class <EntityName>Configuration : IEntityTypeConfiguration<<EntityName>>
{
    public void Configure(EntityTypeBuilder<<EntityName>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("<table_name>");  // snake_case plural

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new <EntityName>Id(value));

        builder.Property(x => x.CreatedAt).HasPrecision(0).IsRequired();
        builder.Property(x => x.UpdatedAt).HasPrecision(0).IsRequired();
        builder.Property(x => x.DeletedAt).HasPrecision(0);

        builder.HasQueryFilter(x => !x.DeletedAt.HasValue);  // soft delete

        builder.Ignore(x => x.IsDeleted);
        builder.Ignore(x => x.DomainEvents);  // only for aggregates
    }
}
```

## Rules
- Table name in `ToTable` must be snake_case plural (e.g., `Entity` → `"entities"`)
- Schema is set via `modelBuilder.HasDefaultSchema(...)` in DbContext — don't set per-table
- `HasPrecision(0)` on ALL `DateTimeOffset` columns — prevents microsecond rounding issues in Postgres
- `HasQueryFilter` for soft delete only if entity implements `ISoftDeletable`
- After generating, remind me to:
  1. Add `DbSet<<EntityName>> <EntityName>s` to `<Module>DbContext`
  2. Register `services.AddScoped<I<EntityName>Repository, <EntityName>Repository>()` in `PersistenceServiceInstaller`
  3. Run `/gen-migration` to create the migration
