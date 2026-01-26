using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Auth;
using Modules.Users.Infrastructure.Security;
using SharedKernel.Infrastructure;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var pepper = configuration["Security:PasswordPepper"];
        
        services.AddSingleton<IPasswordHasher>(_ => new PasswordHasher(pepper));
        services.AddScoped<IJwtProvider, JwtProvider>();
        
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
            });
        services.AddAuthorization();
    }
}