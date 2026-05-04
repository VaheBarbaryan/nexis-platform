using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure;

namespace Modules.Notifications.Infrastructure.ServiceInstallers;

[SuppressMessage("Usage", "CA1812", Justification = "Used via IServiceInstaller collection")]
internal sealed class ValidationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(
            Endpoints.NotificationsEndpointsAssembly.Assembly,
            includeInternalTypes: true);
    }
}
