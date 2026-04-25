using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Endpoints.Posts.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class GetPostByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/posts/{id:guid}", async (
                Guid id,
                IPostService service,
                CancellationToken cancellationToken) =>
            {
                var post = await service.GetByIdAsync(id, cancellationToken);

                return Results.Ok(new PostResponse(
                    post.Id,
                    post.Author,
                    post.Content,
                    post.CreatedAt,
                    post.UpdatedAt,
                    post.LikesCount,
                    post.CommentsCount));
            })
            .WithTags(Tags.Posts)
            .WithName("GetPostById");
    }
}
