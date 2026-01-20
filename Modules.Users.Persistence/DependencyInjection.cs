using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Modules.Users.Persistence.Contexts;
using Modules.Users.Persistence.Interceptors;

namespace Modules.Users.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventsInterceptor>();

        // DbContext
        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("UsersDb");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(5);
                npgsqlOptions.CommandTimeout(30);
            });

            // Snake case naming
            options.UseSnakeCaseNamingConvention();

            // Add interceptor
            options.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
        });

        // services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}