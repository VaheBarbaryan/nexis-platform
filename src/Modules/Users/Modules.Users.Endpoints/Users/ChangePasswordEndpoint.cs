using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/password", async (
                [FromBody] ChangePasswordRequest request,
                IValidator<ChangePasswordRequest> validator,
                IPasswordService passwordService,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? user.FindFirstValue("sub");

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User identification failed",
                        detail: "Unable to extract user identifier from authentication token");
                }

                if (!Guid.TryParse(userId, out var parsedUserId))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid user identifier",
                        detail: "User identifier is not a valid GUID");
                }

                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await passwordService.ChangePasswordAsync(
                    parsedUserId,
                    request.CurrentPassword,
                    request.NewPassword,
                    cancellationToken);

                return Results.Ok(new
                {
                    message = "Password changed successfully!"
                });
            })
            .RequireAuthorization()
            .WithTags(Tags.Users)
            .WithName("ChangePassword");
    }
}
