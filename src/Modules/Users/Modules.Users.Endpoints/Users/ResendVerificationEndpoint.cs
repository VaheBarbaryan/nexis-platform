using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class ResendVerificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resend-verification", async (
                [FromBody] ResendVerificationRequest request,
                IValidator<ResendVerificationRequest> validator,
                IEmailVerificationService emailVerificationService,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await emailVerificationService.ResendEmailVerificationAsync(request.Email, cancellationToken);

                return Results.Ok(new { message = "If an account exists, a verification email has been sent." });
            })
            .WithTags(Tags.Users)
            .WithName("ResendVerification");
    }
}
