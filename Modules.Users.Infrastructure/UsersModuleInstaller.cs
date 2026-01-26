using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application;
using Modules.Users.Persistence;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Extensions;

namespace Modules.Users.Infrastructure;

public sealed class UsersModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.InstallServicesFromAssemblies(
            configuration, 
            UsersInfrastructureAssembly.Assembly,
            UsersApplicationAssembly.Assembly,
            UsersPersistenceAssembly.Assembly);
    }
}