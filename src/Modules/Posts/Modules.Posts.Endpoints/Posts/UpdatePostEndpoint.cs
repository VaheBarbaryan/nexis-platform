using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Endpoints.Posts.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class UpdatePostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/posts/{id:guid}", async (
                Guid id,
                ICurrentUser currentUser,
                [FromBody] UpdatePostRequest request,
                IValidator<UpdatePostRequest> validator,
                IPostService service,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var updatedPost =
                    await service.UpdateAsync(currentUser.Id!.Value, id, request.Content, cancellationToken);

                return Results.Ok(updatedPost);
            })
            .RequireAuthorization()
            .WithTags(Tags.Posts)
            .WithName("UpdatePost");
    }
}
