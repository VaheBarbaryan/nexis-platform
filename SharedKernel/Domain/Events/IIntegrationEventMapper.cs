namespace SharedKernel.Domain.Events;

public interface IIntegrationEventMapper<in TDomainEvent> where TDomainEvent : DomainEvent
{
    object Map(TDomainEvent domainEvent);
}
