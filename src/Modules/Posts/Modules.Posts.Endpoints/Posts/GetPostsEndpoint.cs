using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class GetPostsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("posts", async (
                IPostService service,
                string? cursor,
                CancellationToken cancellationToken,
                int limit = 10) =>
            {
                var posts = await service.GetPostsAsync(cursor, limit, cancellationToken);

                return Results.Ok(posts);
            })
            .WithTags(Tags.Posts)
            .WithName("GetPosts");
    }
}
