using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Modules.Users.Application.Contracts;
using Modules.Users.Application.Seed;
using Modules.Users.Persistence.Contexts;
using Modules.Users.Persistence.Repositories;
using SharedKernel.Application;
using SharedKernel.Infrastructure;

namespace Modules.Users.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IModuleSeeder>(sp => sp.GetRequiredService<RolePermissionSeeder>());
        
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
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<RolePermissionSeeder>();

        return services;
    }
}