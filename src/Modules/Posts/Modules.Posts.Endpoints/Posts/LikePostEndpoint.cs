using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class LikePostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/posts/{id:guid}/like", async (
                ICurrentUser currentUser,
                Guid id,
                IPostService service,
                CancellationToken cancellationToken) =>
            {
                await service.LikeAsync(currentUser.Id!.Value, id, cancellationToken);

                return Results.Created();
            })
            .RequireAuthorization()
            .WithTags(Tags.Posts)
            .WithName("LikePost");
    }
}
