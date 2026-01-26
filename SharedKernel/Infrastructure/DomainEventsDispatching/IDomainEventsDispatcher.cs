namespace SharedKernel.Infrastructure.DomainEventsDispatching;

public interface IDomainEventsDispatcher
{
    Task DispatchEventsAsync();
}