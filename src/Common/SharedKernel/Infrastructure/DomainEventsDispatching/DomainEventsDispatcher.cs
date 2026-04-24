using Autofac;
using Autofac.Core;
using MediatR;
using Newtonsoft.Json;
using SharedKernel.Application.Events;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.Outbox;
using SharedKernel.Infrastructure.Serialization;

namespace SharedKernel.Infrastructure.DomainEventsDispatching;

public sealed class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILifetimeScope _scope;
    private readonly IOutbox _outboxAccessor;
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IDomainNotificationsMapper _domainNotificationsMapper;

    public DomainEventsDispatcher(
        IMediator mediator,
        ILifetimeScope scope,
        IDomainEventsAccessor domainEventsAccessor,
        IDomainNotificationsMapper domainNotificationsMapper,
        IOutbox outboxAccessor)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        _domainEventsAccessor = domainEventsAccessor ?? throw new ArgumentNullException(nameof(domainEventsAccessor));
        _domainNotificationsMapper = domainNotificationsMapper ??
                                     throw new ArgumentNullException(nameof(domainNotificationsMapper));
        _outboxAccessor = outboxAccessor ?? throw new ArgumentNullException(nameof(outboxAccessor));
    }

    public async Task DispatchEventsAsync()
    {
        var domainEvents = _domainEventsAccessor.GetAllDomainEvents();

        List<IDomainEventNotification<IDomainEvent>> domainEventNotifications = [];
        foreach (var domainEvent in domainEvents)
        {
            Type domainEvenNotificationType = typeof(IDomainEventNotification<>);
            var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
            var domainNotification = _scope.ResolveOptional(domainNotificationWithGenericType, new List<Parameter>
            {
                new NamedParameter("domainEvent", domainEvent),
                new NamedParameter("id", domainEvent.EventId)
            });

            if (domainNotification is IDomainEventNotification<IDomainEvent> notification)
            {
                domainEventNotifications.Add(notification);
            }
        }

        _domainEventsAccessor.ClearAllDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }

        foreach (var domainEventNotification in domainEventNotifications)
        {
            var type = _domainNotificationsMapper.GetName(domainEventNotification.GetType());
            var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver(),
                TypeNameHandling = TypeNameHandling.None
            });

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = domainEventNotification.DomainEvent.OccurredOnUtc,
                Type = type!,
                Content = data
            };

            _outboxAccessor.Add(outboxMessage);
        }
    }
}
