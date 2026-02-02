using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans given assemblies for classes and automatically registers them in DI container
    /// </summary>
    public static IServiceCollection InstallServicesFromAssemblies(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var installers = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IServiceInstaller).IsAssignableFrom(t) &&
                t is { IsClass: true, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var installer in installers)
        {
            installer.Install(services, configuration);
        }

        return services;
    }

    /// <summary>
    /// Scans given assemblies for classes implementing IModuleInstaller
    /// and executes their Install method automatically
    /// </summary>
    public static IServiceCollection InstallModulesFromAssemblies(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(assemblies);

        if (assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));
        }

        foreach (var assembly in assemblies)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var installers = assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    typeof(IModuleInstaller).IsAssignableFrom(t))
                .Select(t => (IModuleInstaller)Activator.CreateInstance(t)!);

            foreach (var installer in installers)
            {
                installer.Install(services, configuration);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers a class as all its implemented interfaces
    /// Example: class FooService : IFooService, IBarService
    /// </summary>
    public static IServiceCollection AddTransientAsMatchingInterfaces(
        this IServiceCollection services,
        Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(implementationType);

        var interfaces = implementationType.GetInterfaces();

        foreach (var @interface in interfaces)
        {
            services.AddTransient(@interface, implementationType);
        }

        return services;
    }

    public static IServiceCollection AddScopedAsMatchingInterfaces(
        this IServiceCollection services,
        Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(implementationType);

        var interfaces = implementationType.GetInterfaces();

        foreach (var @interface in interfaces)
        {
            services.AddScoped(@interface, implementationType);
        }

        return services;
    }

    public static IServiceCollection AddSingletonAsMatchingInterfaces(
        this IServiceCollection services,
        Type implementationType)
    {
        ArgumentNullException.ThrowIfNull(implementationType);

        var interfaces = implementationType.GetInterfaces();

        foreach (var @interface in interfaces)
        {
            services.AddSingleton(@interface, implementationType);
        }

        return services;
    }
}
