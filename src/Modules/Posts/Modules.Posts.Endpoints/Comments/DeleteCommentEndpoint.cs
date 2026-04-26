using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Comments;

public sealed class DeleteCommentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("comments/{commentId:guid}", async (
                Guid commentId,
                ICurrentUser currentUser,
                ICommentService service,
                CancellationToken cancellationToken) =>
            {
                await service.DeleteAsync(currentUser.Id!.Value, commentId, cancellationToken);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags(Tags.Comments)
            .WithName("DeleteComment");
    }
}
