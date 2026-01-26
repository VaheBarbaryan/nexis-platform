using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Infrastructure.Events;
using Modules.Users.Persistence;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IDomainEventsAccessor, DomainEventsAccessor>();
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
    }
}