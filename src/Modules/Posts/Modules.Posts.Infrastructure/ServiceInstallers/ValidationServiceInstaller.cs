using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Modules.Posts.Infrastructure.ServiceInstallers;

internal sealed class ValidationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(
            Endpoints.PostsEndpointsAssembly.Assembly,
            includeInternalTypes: true);
    }
}
