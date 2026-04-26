using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("forgot-password", async (
                [FromBody] ForgotPasswordRequest request,
                IValidator<ForgotPasswordRequest> validator,
                IPasswordService passwordService,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await passwordService.ForgotPasswordAsync(request.Email, cancellationToken);

                return Results.Ok(new
                {
                    message = "If an account exists with this email, you will receive a password reset link."
                });
            })
            .WithTags(Tags.Users)
            .WithName("ForgotPassword");
    }
}
