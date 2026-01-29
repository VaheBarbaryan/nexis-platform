namespace SharedKernel.Infrastructure.DomainEventsDispatching;

public class DomainNotificationsMapper : IDomainNotificationsMapper
{
    private readonly BiMap<string, Type> _domainNotificationsMap;

    public DomainNotificationsMapper(BiMap<string, Type> domainNotificationsMap)
    {
        _domainNotificationsMap = domainNotificationsMap;
    }

    public string? GetName(Type type)
    {
        return _domainNotificationsMap.TryGetBySecond(type, out var name) ? name : null;
    }

    public Type? GetTypeByName(string name)
    {
        return _domainNotificationsMap.TryGetByFirst(name, out var type) ? type : null;
    }
}
