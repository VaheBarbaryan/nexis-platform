using Autofac;
using Modules.Posts.Application;
using Modules.Posts.Persistence.Outbox;
using SharedKernel.Application.Events;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Autofac;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

namespace Modules.Posts.Infrastructure.Outbox;

public sealed class OutboxModule : Module
{
    private readonly BiMap<string, Type> _domainNotificationsMap = new();

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(PostsApplicationAssembly.Assembly)
            .AsClosedTypesOf(typeof(IDomainEventNotification<>))
            .InstancePerDependency()
            .FindConstructorsWith(new AllConstructorFinder());

        builder.RegisterType<OutboxAccessor>()
            .Keyed<IOutbox>("posts")
            .FindConstructorsWith(new AllConstructorFinder())
            .InstancePerLifetimeScope();

        PopulateDomainNotificationsMap();
        CheckMappings();

        builder.RegisterType<DomainNotificationsMapper>()
            .Keyed<IDomainNotificationsMapper>("posts")
            .FindConstructorsWith(new AllConstructorFinder())
            .WithParameter("domainNotificationsMap", _domainNotificationsMap)
            .SingleInstance();
    }

    private void CheckMappings()
    {
        var domainEventNotifications = PostsApplicationAssembly.Assembly
            .GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(IDomainEventNotification)))
            .ToList();

        List<Type> notMappedNotifications = [];
        foreach (var domainEventNotification in domainEventNotifications)
        {
            _domainNotificationsMap.TryGetBySecond(domainEventNotification, out var name);

            if (name == null)
            {
                notMappedNotifications.Add(domainEventNotification);
            }
        }

        if (notMappedNotifications.Count > 0)
        {
            throw new InvalidOperationException(
                $"Domain Event Notifications not mapped: " +
                string.Join(", ", notMappedNotifications.Select(x => x.FullName))
            );
        }
    }

    private void PopulateDomainNotificationsMap()
    {
        var domainEventNotifications = PostsApplicationAssembly.Assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDomainEventNotification<>)))
            .ToList();

        foreach (var notification in domainEventNotifications)
        {
            if (notification.FullName is null)
                continue;

            _domainNotificationsMap.Add(notification.FullName, notification);
        }
    }
}
