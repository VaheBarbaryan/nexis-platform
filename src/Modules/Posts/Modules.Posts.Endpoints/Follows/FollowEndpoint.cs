using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Follows;

public sealed class FollowEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/follows/{followeeId:guid}", async (
                Guid followeeId,
                ICurrentUser currentUser,
                IFollowService service,
                CancellationToken cancellationToken
            ) =>
            {
                await service.FollowAsync(currentUser.Id!.Value, followeeId, cancellationToken);

                return Results.Created();
            })
            .RequireAuthorization()
            .WithTags(Tags.Follows)
            .WithName("Follow");
    }
}
