using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/refresh-token", async (
                HttpContext httpContext,
                ILoginUserService loginUserService,
                IOptions<JwtOptions> jwtOptions,
                IWebHostEnvironment env,
                CancellationToken cancellationToken = default) =>
            {
                if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
                {
                    return Results.Unauthorized();
                }

                var result = await loginUserService.RefreshTokenAsync(refreshToken, cancellationToken);

                var jwt = jwtOptions.Value;

                var accessCookieOptions = new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = !env.IsDevelopment(),
                    Expires = DateTime.UtcNow.AddMinutes(jwt.AccessTokenExpirationMinutes)
                };

                var refreshCookieOptions = new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = !env.IsDevelopment(),
                    Expires = DateTime.UtcNow.AddDays(jwt.RefreshTokenExpirationDays)
                };

                httpContext.Response.Cookies.Append("AccessToken", result.AccessToken, accessCookieOptions);
                httpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken, refreshCookieOptions);

                return Results.Ok();
            })
            .WithTags(Tags.Users)
            .WithName("Refresh Tokens");
    }
}
