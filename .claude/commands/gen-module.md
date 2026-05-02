Scaffold a complete new module for this modular monolith. Arguments: `$ARGUMENTS`

Expected format: `<ModuleName>`

Example: `Notifications`

This creates the full 6-layer module structure following the Posts/Users conventions exactly.

## Projects to create

```
src/Modules/<ModuleName>/
  Modules.<ModuleName>.Domain/
  Modules.<ModuleName>.Application/
  Modules.<ModuleName>.Persistence/
  Modules.<ModuleName>.Infrastructure/
  Modules.<ModuleName>.Endpoints/
  Modules.<ModuleName>.IntegrationEvents/
```

## Files per project

### Domain project
- `AssemblyReference.cs`
- `I<ModuleName>UnitOfWork.cs` — extends `IUnitOfWork`
- `<ModuleName>Schema.cs` — `public static class <ModuleName>Schema { public const string Name = "<modulename>"; }`

### Application project
- `AssemblyReference.cs`

### Persistence project
- `AssemblyReference.cs`
- `Contexts/<ModuleName>DbContext.cs` — inherits `DbContext`, includes `DbSet<OutboxMessage>`, `HasDefaultSchema(<ModuleName>Schema.Name)`, `ApplyConfigurationsFromAssembly`, soft-delete override
- `<ModuleName>UnitOfWork.cs` — implements `I<ModuleName>UnitOfWork`, takes keyed `IDomainEventsDispatcher("<modulename>")`

### Infrastructure project
- `AssemblyReference.cs`
- `<ModuleName>ModuleInstaller.cs` — implements `IModuleInstaller`, calls `InstallServicesFromAssemblies` with all 3 assemblies
- `ServiceInstallers/PersistenceServiceInstaller.cs` — DbContext (Npgsql + snake_case + migrations), DomainEventsDispatcher keyed registration
- `ServiceInstallers/OutboxServiceInstaller.cs` — OutboxProcessor BackgroundService, keyed IOutbox, IDomainNotificationsMapper
- `ServiceInstallers/EndpointsServiceInstaller.cs` — scans Endpoints assembly for IEndpoint
- `ServiceInstallers/ValidationServiceInstaller.cs` — FluentValidation from Endpoints assembly
- `ServiceInstallers/KafkaServiceInstaller.cs` — IEventBusPublisher (if module publishes events)

### Endpoints project
- `AssemblyReference.cs`
- `Tags.cs` — `public static class Tags { public const string <ModuleName> = "<ModuleName>"; }`

### IntegrationEvents project
- `AssemblyReference.cs`

## Key patterns (copy exactly)

**AssemblyReference.cs:**
```csharp
namespace Modules.<ModuleName>.<Layer>;

public sealed class <ModuleName><Layer>Assembly
{
    public static readonly System.Reflection.Assembly Assembly = typeof(<ModuleName><Layer>Assembly).Assembly;
}
```

**ModuleInstaller:**
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.<ModuleName>.Application;
using Modules.<ModuleName>.Persistence;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Extensions;

namespace Modules.<ModuleName>.Infrastructure;

public sealed class <ModuleName>ModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.InstallServicesFromAssemblies(
            configuration,
            <ModuleName>InfrastructureAssembly.Assembly,
            <ModuleName>ApplicationAssembly.Assembly,
            <ModuleName>PersistenceAssembly.Assembly);
    }
}
```

**PersistenceServiceInstaller (keyed DomainEventsDispatcher):**
```csharp
services.AddKeyedScoped<IDomainEventsDispatcher>("<modulename>", (sp, _) =>
{
    var mediator = sp.GetRequiredService<IMediator>();
    var scope = sp.GetRequiredService<ILifetimeScope>();
    var mapper = sp.GetRequiredKeyedService<IDomainNotificationsMapper>("<modulename>");
    var outbox = sp.GetRequiredKeyedService<IOutbox>("<modulename>");
    var dbContext = sp.GetRequiredService<<ModuleName>DbContext>();
    return new DomainEventsDispatcher(mediator, scope, new DomainEventsAccessor(dbContext), mapper, outbox);
});
```

## Post-generation checklist

After generating all files I will remind you to:
1. Add `<ProjectReference>` entries for each project to reference their dependencies (Domain ← Application ← Persistence ← Infrastructure; SharedKernel everywhere)
2. Add all 6 `.csproj` files to `nexis-platform.sln`
3. Wire up `<ModuleName>ModuleInstaller` — it's auto-discovered via reflection, no manual wiring needed
4. Add connection string in `appsettings.json` if using a separate DB schema
5. Run `/gen-migration Init <ModuleName>` to create the initial migration

## Rules
- Module key (used for keyed DI) is always lowercase module name: `"<modulename>"`
- Do NOT share DbContext across modules — each module owns its schema
- Copy `SuppressMessage("CA1812")` on internal `IServiceInstaller` implementations
- All `IServiceInstaller` classes must be `internal sealed`
- Ask me what entities/aggregates the module will have before writing domain code
