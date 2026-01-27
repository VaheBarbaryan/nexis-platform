using System.Diagnostics.CodeAnalysis;
using MediatR;
using Modules.Users.Domain.Users.Events;

namespace Modules.Users.Application.Events;

[SuppressMessage("Usage", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via DI")]
internal sealed class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine(notification);
        return Task.CompletedTask;
    }
}
