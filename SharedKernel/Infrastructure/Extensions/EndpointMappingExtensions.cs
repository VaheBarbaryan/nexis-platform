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
        // Resolve all registered IEndpoint implementations
        var endpoints = app.Services.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}