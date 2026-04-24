using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Infrastructure.Outbox;
using SharedKernel.Infrastructure;

namespace Modules.Posts.Infrastructure.ServiceInstallers;

internal sealed class OutboxServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<OutboxProcessor>();
    }
}
