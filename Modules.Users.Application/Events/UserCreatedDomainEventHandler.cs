using MediatR;
using Modules.Users.Domain.Users.Events;

namespace Modules.Users.Application.Events;

internal sealed class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("UserCreatedDomainEventHandler It's working!");
        Console.WriteLine(notification);
        return Task.CompletedTask;
    }
}