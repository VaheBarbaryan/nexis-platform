using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Application.Auth;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class MeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("me", (ICurrentUser currentUser) =>
            {
                var response = new UserResponse(
                    Id: currentUser.Id!.Value.ToString(),
                    Email: currentUser.Email,
                    Username: currentUser.Username
                );
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags(Tags.Users)
            .WithName("GetMe");
    }
}
