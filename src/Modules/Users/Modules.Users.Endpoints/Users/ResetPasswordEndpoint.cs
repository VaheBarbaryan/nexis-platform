using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class ResetPasswordEndpoint : IEndpoint
{
    private const string GenericMessage = "If a valid reset link was used, your password has been updated.";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/reset-password", async (
                [FromBody] ResetPasswordRequest request,
                IValidator<ResetPasswordRequest> validator,
                IPasswordService passwordService,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await passwordService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);

                return Results.Ok(new { message = GenericMessage });
            })
            .WithTags(Tags.Users).WithName("ResetPassword");
    }
}
