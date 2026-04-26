using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Follows;

public sealed class GetFollowingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("authors/{authorId:guid}/following", async (
                Guid authorId,
                IFollowService service,
                string? cursor,
                CancellationToken cancellationToken,
                int limit = 20) =>
            {
                var result = await service.GetFollowingAsync(authorId, cursor, limit, cancellationToken);

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithTags(Tags.Follows)
            .WithName("GetFollowing");
    }
}
