using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Application.Auth;
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
                ICurrentUser currentUser,
                CancellationToken cancellationToken) =>
            {
                if (currentUser.Id is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized");
                }

                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await passwordService.ChangePasswordAsync(
                    currentUser.Id.Value,
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
