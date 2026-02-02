using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application;
using Modules.Users.Application.Notifications;
using Modules.Users.Domain.Users.Events;
using Modules.Users.Infrastructure.Outbox;
using Modules.Users.Persistence.Outbox;
using SharedKernel.Application.Events;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Users.Infrastructure.ServiceInstallers;

internal sealed class OutboxServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Background worker
        services.AddHostedService<OutboxProcessor>();
    }
}
