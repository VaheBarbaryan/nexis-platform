using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Application;
using Modules.Posts.Persistence;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Extensions;

namespace Modules.Posts.Infrastructure;

public sealed class PostsModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.InstallServicesFromAssemblies(
            configuration,
            PostsInfrastructureAssembly.Assembly,
            PostsApplicationAssembly.Assembly,
            PostsPersistenceAssembly.Assembly);
    }
}
