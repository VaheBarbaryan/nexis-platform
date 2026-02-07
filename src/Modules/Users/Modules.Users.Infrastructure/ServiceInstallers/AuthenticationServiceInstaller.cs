using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Options;
using Modules.Users.Infrastructure.Auth;
using Modules.Users.Infrastructure.Redis.Implementations;
using Modules.Users.Infrastructure.Security;
using SharedKernel.Infrastructure;

namespace Modules.Users.Infrastructure.ServiceInstallers;

[SuppressMessage("Usage", "CA1812", Justification = "Used via IServiceInstaller collection")]
internal sealed class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var pepper = configuration["Security:PasswordPepper"];

        if (string.IsNullOrWhiteSpace(pepper))
        {
            throw new InvalidOperationException("Security:PasswordPepper configuration value is missing.");
        }

        services.AddSingleton<IPasswordHasher>(_ => new PasswordHasher(pepper));
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();

        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration
                    .GetSection("Jwt")
                    .Get<JwtOptions>()!;

                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.AccessSecret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["AccessToken"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthentication();
        services.AddAuthorization();
    }
}
