Generate a complete Minimal API endpoint for this modular monolith. Arguments: `$ARGUMENTS`

Expected format: `<HttpVerb> <route> <EndpointName> <ModuleName> [--auth] [--body] [--response]`

Examples:
- `POST posts CreatePost Posts --auth --body --response`
- `GET posts/{id:guid} GetPostById Posts --response`
- `DELETE posts/{id:guid} DeletePost Posts --auth`
- `PUT comments/{id:guid} UpdateComment Posts --auth --body --response`

## Files to generate

For module `Modules.<ModuleName>.Endpoints`, under the appropriate feature subfolder (infer from route):

### 1. Endpoint class — `{Feature}/{EndpointName}Endpoint.cs`

Namespace: `Modules.<ModuleName>.Endpoints.<Feature>`

Pattern (with `--auth` and `--body`):
```csharp
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.<ModuleName>.Application.Contracts;
using Modules.<ModuleName>.Endpoints.<Feature>.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.<ModuleName>.Endpoints.<Feature>;

public sealed class <EndpointName>Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.Map<Verb>("<route>", async (
                <params>) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var result = await service.<ServiceMethod>(..., cancellationToken);

                return Results.<StatusCode>(result);
            })
            .RequireAuthorization()   // only if --auth
            .WithTags(Tags.<Feature>)
            .WithName("<EndpointName>");
    }
}
```

No-body GET pattern omits validator and `[FromBody]`. DELETE returns `Results.NoContent()`. POST returns `Results.Created(...)`. GET returns `Results.Ok(...)`.

### 2. Request record — `{Feature}/Contracts/{EndpointName}Request.cs` (only if `--body`)

```csharp
namespace Modules.<ModuleName>.Endpoints.<Feature>.Contracts;

public sealed record <EndpointName>Request(/* properties */);
```

### 3. Response record — `{Feature}/Contracts/{EndpointName}Response.cs` (only if `--response`)

```csharp
namespace Modules.<ModuleName>.Endpoints.<Feature>.Contracts;

public sealed record <EndpointName>Response(/* properties matching the domain model */);
```

### 4. Validator — `{Feature}/Validation/{EndpointName}RequestValidator.cs` (only if `--body`)

```csharp
using FluentValidation;
using Modules.<ModuleName>.Endpoints.<Feature>.Contracts;

namespace Modules.<ModuleName>.Endpoints.<Feature>.Validation;

internal sealed class <EndpointName>RequestValidator : AbstractValidator<<EndpointName>Request>
{
    public <EndpointName>RequestValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty()
            .MaximumLength(500);
    }
}
```

## Rules
- Infer `Tags.<Feature>` from `Tags.cs` in the endpoints project — check existing tags, add new constant if needed
- `ICurrentUser currentUser` param only when `--auth`
- `CancellationToken cancellationToken` always last (or second-to-last before limit for GET-list endpoints)
- Route path params before body: `Guid id` before `[FromBody] request`
- Service interface method must be added to the existing `I<Feature>Service` if it doesn't exist yet
- Existing service interface lives at `src/Modules/<ModuleName>/Modules.<ModuleName>.Application/Contracts/I<Feature>Service.cs`
- Show me all files before writing any; ask for confirmation if the service method signature is unclear
