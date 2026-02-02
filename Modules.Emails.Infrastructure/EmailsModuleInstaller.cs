using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Extensions;

namespace Modules.Emails.Infrastructure;

public sealed class EmailsModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.InstallServicesFromAssemblies(
            configuration,
            EmailsInfrastructureAssembly.Assembly);
    }
}
