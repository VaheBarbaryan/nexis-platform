using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Notifications.Application;
using Modules.Notifications.Persistence;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Extensions;

namespace Modules.Notifications.Infrastructure;

public sealed class NotificationsModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.InstallServicesFromAssemblies(
            configuration,
            NotificationsInfrastructureAssembly.Assembly,
            NotificationsApplicationAssembly.Assembly,
            NotificationsPersistenceAssembly.Assembly);
    }
}
