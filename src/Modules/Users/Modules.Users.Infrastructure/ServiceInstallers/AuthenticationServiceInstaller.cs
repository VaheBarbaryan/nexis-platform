using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Infrastructure.Auth;
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
    }
}
