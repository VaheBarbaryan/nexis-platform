using Modules.Users.Application.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class VerifyUserEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/verify-email", async (
                [FromQuery] string token,
                IEmailVerificationService emailVerificationService,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return Results.BadRequest(new
                    {
                        error = "Token is required"
                    });
                }

                try
                {
                    var user = await emailVerificationService.VerifyEmailAsync(token, cancellationToken);

                    return Results.Ok(new
                    {
                        userId = user.Id.Value,
                        emailVerifiedAt = user.EmailVerifiedAt
                    });
                }
                catch (BadHttpRequestException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithTags(Tags.Users)
            .WithName("VerifyEmail");
    }
}
