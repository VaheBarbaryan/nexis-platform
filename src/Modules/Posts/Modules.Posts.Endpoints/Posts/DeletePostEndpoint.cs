using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class DeletePostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("posts/{id:guid}", async (
                Guid id,
                ICurrentUser currentUser,
                IPostService service,
                CancellationToken cancellationToken) =>
            {
                await service.DeleteAsync(currentUser.Id!.Value, id, cancellationToken);
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags(Tags.Posts)
            .WithName("DeletePost");
    }
}
