namespace SharedKernel.Infrastructure.DomainEventsDispatching;

public interface IDomainNotificationsMapper
{
    string? GetName(Type type);

    Type? GetTypeByName(string name);
}
