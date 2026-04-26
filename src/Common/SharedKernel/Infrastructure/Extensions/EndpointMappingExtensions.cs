using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Endpoints;

namespace SharedKernel.Infrastructure.Extensions;

public static class EndpointMappingExtensions
{
    public static WebApplication MapEndpoints(
        this WebApplication app,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Resolve all registered IEndpoint implementations
        var endpoints = app.Services.GetServices<IEndpoint>();
        var group = app.MapGroup("/api");

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(group);
        }

        return app;
    }
}
