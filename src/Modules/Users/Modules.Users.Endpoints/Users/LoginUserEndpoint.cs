using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;

namespace Modules.Users.Endpoints.Users;

public class LoginUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", async (
                HttpContext httpContext,
                [FromBody] LoginUserRequest request,
                IValidator<LoginUserRequest> validator,
                ILoginUserService loginUserService,
                IOptions<JwtOptions> jwtOptions,
                IWebHostEnvironment env,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await loginUserService.LoginAsync(
                    request.Email,
                    request.Password,
                    cancellationToken);

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
            .WithName("Login");
    }
}
