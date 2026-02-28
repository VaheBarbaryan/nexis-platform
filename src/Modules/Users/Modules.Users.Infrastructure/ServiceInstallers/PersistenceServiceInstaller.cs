using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Seed;
using Modules.Users.Domain.Permissions.Repositories;
using Modules.Users.Domain.Roles.Repositories;
using Modules.Users.Domain.Users.Repositories;
using Modules.Users.Infrastructure.Events;
using Modules.Users.Persistence;
using Modules.Users.Persistence.Contexts;
using Modules.Users.Persistence.Outbox;
using Modules.Users.Persistence.Repositories;
using SharedKernel.Application;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Users.Infrastructure.ServiceInstallers;

[SuppressMessage("Usage", "CA1812", Justification = "Used via IServiceInstaller collection")]
internal sealed class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IModuleSeeder>(sp => sp.GetRequiredService<RolePermissionSeeder>());

        // DbContext
        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("UsersDb");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure();
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, UsersSchema.Name);
            });

            // Snake case naming
            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<RolePermissionSeeder>();

        services.AddScoped<IDomainEventsAccessor, DomainEventsAccessor>();
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
    }
}
