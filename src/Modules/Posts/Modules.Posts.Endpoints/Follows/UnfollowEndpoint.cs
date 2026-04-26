using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Follows;

public sealed class UnfollowEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("follows/{followeeId:guid}", async (
                Guid followeeId,
                ICurrentUser currentUser,
                IFollowService service,
                CancellationToken cancellationToken) =>
            {
                await service.UnfollowAsync(currentUser.Id!.Value, followeeId, cancellationToken);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags(Tags.Follows)
            .WithName("Unfollow");
    }
}
