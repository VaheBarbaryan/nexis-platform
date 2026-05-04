using System.Diagnostics.CodeAnalysis;
using Autofac;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Notifications.Application.Contracts;
using Modules.Notifications.Application.Services;
using Modules.Notifications.Domain;
using Modules.Notifications.Domain.NotificationActors.Repositories;
using Modules.Notifications.Domain.Notifications.Repositories;
using Modules.Notifications.Infrastructure.Events;
using Modules.Notifications.Persistence;
using Modules.Notifications.Persistence.Contexts;
using Modules.Notifications.Persistence.Repositories;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Notifications.Infrastructure.ServiceInstallers;

[SuppressMessage("Usage", "CA1812", Justification = "Used via IServiceInstaller collection")]
internal sealed class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Database");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(NotificationsDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure();
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, NotificationsSchema.Name);
            });

            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<INotificationActorRepository, NotificationActorRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationActorService, NotificationActorService>();
        services.AddScoped<INotificationsUnitOfWork, NotificationsUnitOfWork>();

        services.AddKeyedScoped<IDomainEventsDispatcher>("notifications", (sp, _) =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var scope = sp.GetRequiredService<ILifetimeScope>();
            var mapper = sp.GetRequiredKeyedService<IDomainNotificationsMapper>("notifications");
            var outbox = sp.GetRequiredKeyedService<IOutbox>("notifications");
            var dbContext = sp.GetRequiredService<NotificationsDbContext>();
            return new DomainEventsDispatcher(mediator, scope, new DomainEventsAccessor(dbContext), mapper, outbox);
        });
    }
}
