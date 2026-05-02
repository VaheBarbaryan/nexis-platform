Generate a domain entity or aggregate root with its value object and exception. Arguments: `$ARGUMENTS`

Expected format: `<EntityName> <Module> [aggregate|entity]`

Default is `aggregate` if not specified.

Examples:
- `Story Posts aggregate`
- `Tag Posts entity`

## Files to generate

### 1. Value object — `src/Modules/<Module>/Modules.<Module>.Domain/<EntityName>/ValueObjects/<EntityName>Id.cs`

```csharp
namespace Modules.<Module>.Domain.<EntityName>.ValueObjects;

public sealed record <EntityName>Id(Guid Value);
```

### 2a. Aggregate root — `src/Modules/<Module>/Modules.<Module>.Domain/<EntityName>/<EntityName>.cs` (when `aggregate`)

```csharp
using Modules.<Module>.Domain.<EntityName>.ValueObjects;
using SharedKernel.Domain.Aggregates;
using SharedKernel.Domain.Entities;

namespace Modules.<Module>.Domain.<EntityName>;

public sealed class <EntityName> : AggregateRoot<<EntityName>Id>, ISoftDeletable
{
    // public properties with private setters
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    private <EntityName>() { }  // EF Core

    private <EntityName>(/* params */)
    {
        // CheckRule(...) for invariants
        Id = new <EntityName>Id(Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static <EntityName> Create(/* params */)
    {
        var entity = new <EntityName>(/* params */);
        entity.RaiseDomainEvent(new <EntityName>CreatedDomainEvent(/* ... */));
        return entity;
    }

    // Update / Delete methods follow same pattern
}
```

### 2b. Child entity — `src/Modules/<Module>/Modules.<Module>.Domain/<EntityName>/<EntityName>.cs` (when `entity`)

```csharp
using Modules.<Module>.Domain.<EntityName>.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.<Module>.Domain.<EntityName>;

public sealed class <EntityName> : Entity<<EntityName>Id>
{
    // public properties with private setters

    private <EntityName>() { }  // EF Core

    private <EntityName>(/* params */)
    {
        Id = new <EntityName>Id(Guid.NewGuid());
    }

    public static <EntityName> Create(/* params */)
    {
        return new <EntityName>(/* params */);
    }
}
```

### 3. NotFoundException — `src/Modules/<Module>/Modules.<Module>.Domain/<EntityName>/Exceptions/<EntityName>NotFoundException.cs`

```csharp
using SharedKernel.Domain.Exceptions;

namespace Modules.<Module>.Domain.<EntityName>.Exceptions;

public sealed class <EntityName>NotFoundException : NotFoundException
{
    public <EntityName>NotFoundException()
        : base("<EntityName> was not found.")
    {
    }
}
```

## Rules
- Aggregate gets `ISoftDeletable` + `AggregateRoot<TId>` + domain events on Create/Update/Delete
- Entity gets `Entity<TId>` only — no domain events, no soft delete unless explicitly requested
- Private parameterless constructor required for EF Core on both
- All mutable properties: `private set`
- Always `DateTimeOffset` not `DateTime` for timestamps
- Ask me what properties the entity needs before writing if not specified in arguments
- After generating, remind me to: add DbSet to DbContext, add IEntityTypeConfiguration, register repository in DI
