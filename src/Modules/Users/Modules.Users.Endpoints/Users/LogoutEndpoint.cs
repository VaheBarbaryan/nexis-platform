using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Users.Application.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/logout", async (
                HttpContext httpContext,
                [FromKeyedServices(TokenStoreKey.RefreshToken)] ITokenStore<Guid> refreshTokenStore,
                ITokenGenerator tokenGenerator,
                IWebHostEnvironment env,
                CancellationToken cancellationToken = default) =>
            {
                if (httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
                {
                    var hashedRefreshToken = tokenGenerator.Hash(refreshToken);
                    await refreshTokenStore.RemoveAsync(hashedRefreshToken, cancellationToken);
                }

                httpContext.Response.Cookies.Delete("AccessToken", new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = !env.IsDevelopment(),
                });

                httpContext.Response.Cookies.Delete("RefreshToken", new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = !env.IsDevelopment(),
                });

                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags(Tags.Users)
            .WithName("Logout");
    }
}
