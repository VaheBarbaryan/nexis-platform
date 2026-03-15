using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Endpoints.Posts.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Posts.Endpoints.Posts;

public sealed class CreatePostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/posts", async (
                ClaimsPrincipal author,
                [FromBody] CreatePostRequest request,
                IValidator<CreatePostRequest> validator,
                IPostService service,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var authorId = author.FindFirstValue(ClaimTypes.NameIdentifier)
                               ?? author.FindFirstValue("sub");

                if (!Guid.TryParse(authorId, out var parsedAuthorId))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid user identifier",
                        detail: "User identifier is not a valid GUID");
                }

                var result = await service.CreateAsync(parsedAuthorId, request.Content, cancellationToken);

                return Results.Created("/api/posts", result);
            })
            .RequireAuthorization()
            .WithTags(Tags.Posts)
            .WithName("CreatePost");
    }
}
