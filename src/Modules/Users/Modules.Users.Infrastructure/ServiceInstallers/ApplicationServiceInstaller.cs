using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Services;
using Modules.Users.Infrastructure.Configuration;
using Modules.Users.Infrastructure.Services;
using SharedKernel.Infrastructure;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection("Frontend"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IVerificationLinkBuilder, VerificationLinkBuilder>();
        services.AddScoped<IRegisterUserService, RegisterUserService>();
    }
}
