using Modules.Notifications.Application.Contracts;
using Modules.Notifications.Domain;
using Modules.Notifications.Domain.NotificationActors;
using Modules.Notifications.Domain.NotificationActors.Repositories;
using Modules.Notifications.Domain.NotificationActors.ValueObjects;

namespace Modules.Notifications.Application.Services;

public sealed class NotificationActorService : INotificationActorService
{
    private readonly INotificationActorRepository _notificationActorRepository;
    private readonly INotificationsUnitOfWork _unitOfWork;

    public NotificationActorService(
        INotificationActorRepository notificationActorRepository,
        INotificationsUnitOfWork unitOfWork)
    {
        _notificationActorRepository = notificationActorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationActor> CreateAsync(Guid userId, string username, CancellationToken ct)
    {
        var actorId = NotificationActorId.From(userId);

        var notificationActor = NotificationActor.Create(actorId, NotificationUsername.From(username));

        if (await _notificationActorRepository.ExistsAsync(actorId, ct))
        {
            return notificationActor;
        }

        _notificationActorRepository.Add(notificationActor);

        await _unitOfWork.CommitAsync(ct);
        return notificationActor;
    }
}
