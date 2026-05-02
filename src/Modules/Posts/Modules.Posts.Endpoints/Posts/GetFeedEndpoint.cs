using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class GetFeedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("posts/feed", async (
                IPostService service,
                ICurrentUser currentUser,
                string? cursor,
                CancellationToken cancellationToken,
                int limit = 10) =>
            {
                var feed = await service.GetFeedAsync(currentUser.Id!.Value, cursor, limit, cancellationToken);

                return Results.Ok(feed);
            })
            .RequireAuthorization()
            .WithTags(Tags.Posts)
            .WithName("GetFeed");
    }
}
