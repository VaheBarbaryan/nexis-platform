using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class MeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/me", (ClaimsPrincipal user) =>
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

                var response = new UserResponse(
                    Id: userId,
                    Email: user.FindFirstValue("email"),
                    Username: user.FindFirstValue("username")
                );
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags(Tags.Users)
            .WithName("GetMe");
    }
}
