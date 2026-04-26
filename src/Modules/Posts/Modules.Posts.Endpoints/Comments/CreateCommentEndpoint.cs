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

public sealed class CreateCommentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("posts/{postId:guid}/comments", async (
                Guid postId,
                ICurrentUser currentUser,
                [FromBody] CreateCommentRequest request,
                IValidator<CreateCommentRequest> validator,
                ICommentService service,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await service.CreateAsync(currentUser.Id!.Value, postId, request.Content, cancellationToken);

                return Results.Created(
                    $"/posts/{postId}/comments/{result.Id}",
                    new CommentResponse(result.Id, result.PostId, result.Author, result.Content, result.CreatedAt, result.UpdatedAt));
            })
            .RequireAuthorization()
            .WithTags(Tags.Comments)
            .WithName("CreateComment");
    }
}
