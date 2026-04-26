using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Endpoints.Comments.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Comments;

public sealed class UpdateCommentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("comments/{commentId:guid}", async (
                Guid commentId,
                ICurrentUser currentUser,
                [FromBody] UpdateCommentRequest request,
                IValidator<UpdateCommentRequest> validator,
                ICommentService service,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await service.UpdateAsync(currentUser.Id!.Value, commentId, request.Content,
                    cancellationToken);

                return Results.Ok(
                    new CommentResponse(result.Id, result.PostId, result.Author, result.Content,
                        result.CreatedAt, result.UpdatedAt));
            })
            .RequireAuthorization()
            .WithTags(Tags.Comments)
            .WithName("UpdateComment");
    }
}
