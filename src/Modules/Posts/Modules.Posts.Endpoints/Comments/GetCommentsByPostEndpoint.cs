using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Comments;

public sealed class GetCommentsByPostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/posts/{postId:guid}/comments", async (
                Guid postId,
                ICommentService service,
                string? cursor,
                CancellationToken cancellationToken,
                int limit = 20) =>
            {
                var comments = await service.GetByPostIdAsync(postId, cursor, limit, cancellationToken);

                return Results.Ok(comments);
            })
            .WithTags(Tags.Comments)
            .WithName("GetCommentsByPost");
    }
}
